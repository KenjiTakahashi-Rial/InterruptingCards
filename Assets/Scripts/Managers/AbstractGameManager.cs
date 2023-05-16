using System;
using System.Collections.Generic;

using TMPro;
using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Behaviours;
using InterruptingCards.Config;
using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public abstract class AbstractGameManager : NetworkBehaviour
    {
        /******************************************************************************************\
         * Constants                                                                              *
        \******************************************************************************************/

        protected const int GameStateMachineLayer = 0;

        protected const string WaitingForClientsStateName = "Base.WaitingForClients";
        protected const string WaitingForAllReadyStateName = "Base.WaitingForAllReady";

        protected const string EndGameStateName = "Base.InGame.EndGame";

        protected const string StartTurnStateName = "Base.InGame.PlayerTurns.StartTurn";
        protected const string WaitingForDrawCardStateName = "Base.InGame.PlayerTurns.WaitingForDrawCard";
        protected const string WaitingForPlayCardStateName = "Base.InGame.PlayerTurns.WaitingForPlayCard";
        protected const string EndTurnStateName = "Base.InGame.PlayerTurns.EndTurn";

        protected const string DrawCardTriggerName = "drawCard";
        protected const string ForceEndTurnTriggerName = "forceEndTurn";
        protected const string ForceEndGameTriggerName = "forceEndGame";
        protected const string PlayCardTriggerName = "playCard";
        protected const string StartGameTriggerName = "startGame";
        protected const string WaitForReadyTriggerName = "waitForReady";

        /******************************************************************************************\
         * Fields                                                                                 *
        \******************************************************************************************/

        protected static readonly int WaitingForClientsStateId = Animator.StringToHash(WaitingForClientsStateName);
        protected static readonly int WaitingForAllReadyStateId = Animator.StringToHash(WaitingForAllReadyStateName);

        protected static readonly int EndGameStateId = Animator.StringToHash(EndGameStateName);

        protected static readonly int StartTurnStateId = Animator.StringToHash(StartTurnStateName);
        protected static readonly int WaitingForDrawCardStateId = Animator.StringToHash(WaitingForDrawCardStateName);
        protected static readonly int WaitingForPlayCardStateId = Animator.StringToHash(WaitingForPlayCardStateName);
        protected static readonly int EndTurnStateId = Animator.StringToHash(EndTurnStateName);

        protected static readonly int DrawCardTriggerId = Animator.StringToHash(DrawCardTriggerName);
        protected static readonly int ForceEndTurnTriggerId = Animator.StringToHash(ForceEndTurnTriggerName);
        protected static readonly int ForceEndGameTriggerId = Animator.StringToHash(ForceEndGameTriggerName);
        protected static readonly int PlayCardTriggerId = Animator.StringToHash(PlayCardTriggerName);
        protected static readonly int StartGameTriggerId = Animator.StringToHash(StartGameTriggerName);
        protected static readonly int WaitForReadyTriggerId = Animator.StringToHash(WaitForReadyTriggerName);


        protected readonly Dictionary<int, string> _stateMachineIdNameMap = new()
        {
            { WaitingForClientsStateId, WaitingForClientsStateName },
            { WaitingForAllReadyStateId, WaitingForAllReadyStateName },

            { EndGameStateId, EndGameStateName },

            { StartTurnStateId, StartTurnStateName },
            { WaitingForDrawCardStateId, WaitingForDrawCardStateName },
            { WaitingForPlayCardStateId, WaitingForPlayCardStateName },
            { EndTurnStateId, EndTurnStateName },

            { DrawCardTriggerId, DrawCardTriggerName },
            { ForceEndTurnTriggerId, ForceEndTurnTriggerName },
            { ForceEndGameTriggerId, ForceEndGameTriggerName },
            { PlayCardTriggerId, PlayCardTriggerName },
            { StartGameTriggerId, StartGameTriggerName },
            { WaitForReadyTriggerId, WaitForReadyTriggerName },
        };

        protected readonly PlayerFactory _playerFactory = PlayerFactory.Singleton;
        protected readonly CardFactory _cardFactory = CardFactory.Singleton;
        protected readonly DeckFactory _deckFactory = DeckFactory.Singleton;

        // Server-only
        protected readonly List<ulong> _lobby = new();
        protected readonly HashSet<ulong> _notReadyPlayers = new();
        
        protected readonly LinkedList<Player> _players = new();
        protected LinkedListNode<Player> _activePlayerNode;

        [SerializeField] protected CardPack _cardPack;
        [SerializeField] protected Animator _gameStateMachine;
        [SerializeField] protected DeckManager _deckManager;
        [SerializeField] protected DeckManager _discardManager;
        [SerializeField] protected HandManager[] _handManagers;
        [SerializeField] protected TextMeshPro _tempInfoText;

        protected ulong _selfId; // The player that is on this device

        /******************************************************************************************\
         * Properties                                                                             *
        \******************************************************************************************/

        public static AbstractGameManager Singleton { get; protected set; }

        public string CurrentStateName
        {
            get => _stateMachineIdNameMap.GetValueOrDefault(CurrentStateId, "UnknownState");
        }

        protected virtual int MinPlayers => 2;

        protected virtual int MaxPlayers => 2;

        protected virtual int StartingHandCardCount => 4;

        protected virtual int CurrentStateId =>
            _gameStateMachine.GetCurrentAnimatorStateInfo(GameStateMachineLayer).fullPathHash;

        /******************************************************************************************\
         * Public Methods                                                                         *
        \******************************************************************************************/
        public override void OnNetworkSpawn()
        {
            Debug.Log("Network spawned");

            GetSelfServerRpc();

            if (IsServer)
            {
                NetworkManager.OnClientConnectedCallback -= AddPlayerServerRpc;
                NetworkManager.OnClientConnectedCallback += AddPlayerServerRpc;
                NetworkManager.OnClientDisconnectCallback -= RemovePlayerServerRpc;
                NetworkManager.OnClientDisconnectCallback += RemovePlayerServerRpc;
            }

            _deckManager.OnDeckClicked -= TryDrawCard;
            _deckManager.OnDeckClicked += TryDrawCard;

            foreach (var handManager in _handManagers)
            {
                handManager.OnCardClicked -= TryPlayCard;
                handManager.OnCardClicked += TryPlayCard;
            }
        }

        public override void OnNetworkDespawn()
        {
            Debug.Log("Network despawned");

            if (IsServer)
            {
                _players.Clear();
                NetworkManager.OnClientConnectedCallback -= AddPlayerServerRpc;
                NetworkManager.OnClientDisconnectCallback -= RemovePlayerServerRpc;
            }

            _activePlayerNode = null;

            _deckManager.OnDeckClicked -= TryDrawCard;

            foreach (var handManager in _handManagers)
            {
                handManager.OnCardClicked -= TryPlayCard;
            }

            if (CurrentStateId != WaitingForClientsStateId)
            {
                _gameStateMachine.SetTrigger(ForceEndGameTriggerId);
            }
        }

        public virtual void HandleInGame()
        {
            Debug.Log("Starting Game");

            AssignHands();
            
            if (IsServer)
            {
                _deckManager.Initialize(_cardPack);
                _deckManager.Shuffle();
                _deckManager.IsFaceUp = false;
                _discardManager.Clear();
                DealHandsServerRpc();
            }

            _activePlayerNode = _players.First;
        }

        public abstract void HandleStartTurn();

        public virtual void HandleEndTurn(int shifts = 1)
        {
            Debug.Log($"Shifting turn {shifts} times");

            for (var i = 0; i < shifts; i++)
            {
                _activePlayerNode = _activePlayerNode.Next ?? _players.First;
            }
        }

        public virtual void HandleEndGame()
        {
            Debug.Log("Ending game");

            _players.Clear();
            _activePlayerNode = null;
            _tempInfoText.SetText("Start the game");

            _deckManager.Clear();
            _discardManager.Clear();

            foreach (var handManager in _handManagers)
            {
                handManager.Clear();
            }
        }

        /******************************************************************************************\
         * Protected Methods                                                                      *
        \******************************************************************************************/

        protected virtual void Awake()
        {
            Debug.Log("Waking game manager");

            _cardFactory.Load(_cardPack);
            _deckFactory.Load(_cardPack);

            Singleton = this;
        }

        protected virtual new void OnDestroy()
        {
            Debug.Log("Destroying game manager");

            Singleton = null;
            _players.Clear();
            base.OnDestroy();
        }

        protected virtual void Update()
        {
            if (_activePlayerNode == null || _tempInfoText == null)
            {
                return;
            }

            _tempInfoText.SetText($"{_activePlayerNode.Value.Id}\n{CurrentStateName}");
        }

        [ClientRpc]
        protected virtual void StateTriggerClientRpc(int triggerId, ClientRpcParams clientRpcParams = default)
        {
            Debug.Log($"Triggering {_stateMachineIdNameMap.GetValueOrDefault(triggerId, "UnknownTrigger")}");
            _gameStateMachine.SetTrigger(triggerId);
        }

        [ServerRpc(RequireOwnership = false)]
        protected virtual void GetSelfServerRpc(ServerRpcParams serverRpcParams = default)
        {
            var clientId = serverRpcParams.Receive.SenderClientId;

            Debug.Log($"Getting self {clientId}");

            var clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            };

            AssignSelfClientRpc(clientId, clientRpcParams);
        }

        [ClientRpc]
        protected virtual void AssignSelfClientRpc(ulong selfId, ClientRpcParams clientRpcParams = default)
        {
            Debug.Log($"Assigning self {selfId}");
            _selfId = selfId;
        }

        protected virtual void AssignHands()
        {
            if (_players.Count > _handManagers.Length)
            {
                throw new TooManyPlayersException();
            }

            if (!IsServer)
            {
                return;
            }

            Debug.Log("Assigning hands");

            var i = 0;
            foreach (var player in _players)
            {
                player.Hand = _handManagers[i++].Hand;
            }
        }

        protected virtual void HandleEffect(CardActiveEffect effect)
        {
            switch (effect)
            {
                case CardActiveEffect.PlayCard:
                    Debug.Log("PlayCard active effect");
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        [ServerRpc]
        protected virtual void AddPlayerServerRpc(ulong clientId)
        {
            Debug.Log($"Adding player {clientId}");

            if (CurrentStateId != WaitingForClientsStateId)
            {
                Debug.LogWarning($"Cannnot add player {clientId} while game already started");
                return;
            }

            if (_lobby.Count >= MaxPlayers)
            {
                Debug.LogWarning($"Cannnot add player {clientId} while lobby is full");
                return;
            }

            _lobby.Add(clientId);

            if (_lobby.Count == MaxPlayers)
            {
                _notReadyPlayers.UnionWith(_lobby);
                StateTriggerClientRpc(WaitForReadyTriggerId);
                SetPlayersClientRpc(_lobby.ToArray());
            }
        }

        [ServerRpc]
        protected virtual void RemovePlayerServerRpc(ulong clientId)
        {
            Debug.Log($"Removing player {clientId}");

            if (CurrentStateId != WaitingForClientsStateId)
            {
                if (clientId == _activePlayerNode.Value.Id)
                {
                    StateTriggerClientRpc(ForceEndTurnTriggerId);
                }

                Debug.LogWarning($"Player {clientId} removed while game is playing");
            }

            // TODO: Continue game if enough players left
            StateTriggerClientRpc(ForceEndGameTriggerId);
            _lobby.Remove(clientId);
        }

        [ClientRpc]
        protected virtual void SetPlayersClientRpc(ulong[] playerIds, ClientRpcParams clientRpcParams = default)
        {
            foreach (var playerId in playerIds)
            {
               _players.AddLast(_playerFactory.Create(playerId, playerId.ToString()));
            }

            PlayerReadyServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        protected virtual void PlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
        {
            _notReadyPlayers.Remove(serverRpcParams.Receive.SenderClientId);
            
            if (_notReadyPlayers.Count == 0)
            {
                StateTriggerClientRpc(StartGameTriggerId);
            }
        }

        [ServerRpc]
        protected virtual void DealHandsServerRpc()
        {
            if (_players.Count < MinPlayers)
            {
                Debug.Log($"Cannot deal hands before {MinPlayers} joined");
                return;
            }

            if (_activePlayerNode != null)
            {
                Debug.Log("Cannot deal hands during a game");
                return;
            }

            Debug.Log("Dealing hands");

            for (var i = 0; i < StartingHandCardCount; i++)
            {
                foreach (var hand in _handManagers)
                {
                    hand.Add(_deckManager.DrawTop());
                }
            }
        }

        protected virtual bool CanDrawCard(ulong id)
        {
            if (id != _activePlayerNode.Value.Id)
            {
                Debug.Log($"Player {id} cannot draw a card unless it is their turn");
                return false;
            }

            if (CurrentStateId != WaitingForDrawCardStateId)
            {
                Debug.Log($"Player {id} cannot draw a card in the wrong state");
                return false;
            }

            return true;
        }

        protected virtual void TryDrawCard()
        {
            Debug.Log("Trying to draw card");

            if (CanDrawCard(_selfId))
            {
                DrawCardServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        protected virtual void DrawCardServerRpc(ServerRpcParams serverRpcParams = default)
        {
            if (!CanDrawCard(serverRpcParams.Receive.SenderClientId))
            {
                return;
            }

            Debug.Log("Drawing card");

            var card = _deckManager.DrawTop();
            _activePlayerNode.Value.Hand.Add(card);

            StateTriggerClientRpc(DrawCardTriggerId);
        }

        protected virtual bool CanPlayCard(ulong id)
        {
            if (id != _activePlayerNode.Value.Id)
            {
                Debug.Log($"Player {id} cannot play a card unless it is their turn or they are interrupting");
                return false;
            }

            if (CurrentStateId != WaitingForPlayCardStateId)
            {
                Debug.Log($"Player {id} cannot play a card in the wrong state");
                return false;
            }

            return true;
        }

        protected virtual void TryPlayCard(Card card)
        {
            Debug.Log($"Trying to play card {card}");

            if (CanPlayCard(_selfId))
            {
                PlayCardServerRpc(card.Id);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        protected virtual void PlayCardServerRpc(int cardId, ServerRpcParams serverRpcParams = default)
        {
            if (!CanPlayCard(serverRpcParams.Receive.SenderClientId))
            {
                return;
            }

            Debug.Log("Playing card");

            var card = _activePlayerNode.Value.Hand.Remove(cardId);
            _discardManager.PlaceTop(card);

            StateTriggerClientRpc(PlayCardTriggerId);
        }
    }
}
