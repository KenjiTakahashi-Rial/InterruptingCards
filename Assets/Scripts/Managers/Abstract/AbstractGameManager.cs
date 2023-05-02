using System.Collections.Generic;
using System.Linq;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Factories;
using InterruptingCards.Models;
using TMPro;

namespace InterruptingCards.Managers
{
    public abstract class AbstractGameManager : NetworkBehaviour, IGameManager
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

        // Server-only
        protected readonly List<ulong> _lobby = new();
        protected readonly HashSet<ulong> _notReadyPlayers = new();
        
        protected readonly LinkedList<IPlayer> _players = new();
        protected LinkedListNode<IPlayer> _activePlayerNode;

        [SerializeField] protected Animator _gameStateMachine;
        [SerializeField] protected TextMeshPro _tempInfoText;

        protected ulong _selfId; // The player that is on this device

        /******************************************************************************************\
         * Properties                                                                             *
        \******************************************************************************************/

        public static IGameManager Singleton { get; protected set; }

        public string CurrentStateName
        {
            get => _stateMachineIdNameMap.GetValueOrDefault(CurrentStateId, "UnknownState");
        }
        
        protected abstract IPlayerFactory PlayerFactory { get; }

        protected abstract ICardFactory CardFactory { get; }

        protected abstract IHandFactory HandFactory { get; }

        protected abstract IDeckManager DeckManager { get; }

        protected abstract IDeckManager DiscardManager { get; }

        protected abstract IHandManager[] HandManagers { get; }

        protected abstract int MinPlayers { get; }

        protected abstract int MaxPlayers { get; }

        protected abstract int StartingHandCardCount { get; }

        protected virtual bool IsSelfTurn => _activePlayerNode.Value.Id == _selfId;

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
                AddPlayerServerRpc(_selfId);
                NetworkManager.OnClientConnectedCallback -= AddPlayerServerRpc;
                NetworkManager.OnClientConnectedCallback += AddPlayerServerRpc;
                NetworkManager.OnClientDisconnectCallback -= RemovePlayerServerRpc;
                NetworkManager.OnClientDisconnectCallback += RemovePlayerServerRpc;
            }

            DeckManager.OnDeckClicked -= TryDrawCard;
            DeckManager.OnDeckClicked += TryDrawCard;

            foreach (var handManager in HandManagers)
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

            DeckManager.OnDeckClicked -= TryDrawCard;

            foreach (var handManager in HandManagers)
            {
                handManager.OnCardClicked -= TryPlayCard;
            }
        }

        public virtual void StartGame()
        {
            Debug.Log("Starting Game");

            AssignHands();
            
            if (IsServer)
            {
                DeckManager.Initialize();
                DeckManager.IsFaceUp = false;
                DiscardManager.Clear();
                DealHandsServerRpc();
            }

            _activePlayerNode = _players.First;
        }

        public virtual void ShiftTurn(int shifts = 1)
        {
            Debug.Log($"Shifting turn {shifts} times");

            for (var i = 0; i < shifts; i++)
            {
                _activePlayerNode = _activePlayerNode.Next ?? _players.First;
            }
        }

        /******************************************************************************************\
         * Protected Methods                                                                      *
        \******************************************************************************************/

        protected virtual void Awake()
        {
            Debug.Log("Waking game manager");

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

            Debug.Log($"Getting self ({clientId})");

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
            Debug.Log($"Assigning self ({selfId})");
            _selfId = selfId;
        }

        protected virtual void AssignHands()
        {
            Debug.Log("Assigning hands");

            if (_players.Count > HandManagers.Count())
            {
                throw new TooManyPlayersException();
            }

            if (!IsServer)
            {
                return;
            }

            var i = 0;
            foreach (var player in _players)
            {
                HandManagers[i].Hand = HandFactory.Create(new List<ICard>());
                player.Hand = HandManagers[i++];
            }
        }

        [ServerRpc]
        protected virtual void AddPlayerServerRpc(ulong clientId)
        {
            Debug.Log($"Adding player ({clientId})");

            if (CurrentStateId != WaitingForClientsStateId)
            {
                Debug.LogWarning($"Did not add player ({clientId}): game already started");
                return;
            }

            if (_lobby.Count >= MaxPlayers)
            {
                Debug.LogWarning($"Did not add player ({clientId}): lobby is full");
                return;
            }

            _lobby.Add(clientId);
            
            // TODO: Change this to a button or something
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
            Debug.Log($"Removing player ({clientId})");

            if (CurrentStateId != WaitingForClientsStateId)
            {
                if (clientId == _activePlayerNode.Value.Id)
                {
                    StateTriggerClientRpc(ForceEndTurnTriggerId);

                }
                Debug.LogWarning($"Player ({clientId}) removed while game is playing");
            }

            // TODO: Continue game without player
            StateTriggerClientRpc(ForceEndGameTriggerId);
        }

        [ClientRpc]
        protected virtual void SetPlayersClientRpc(ulong[] playerIds, ClientRpcParams clientRpcParams = default)
        {
            foreach (var playerId in playerIds)
            {
               _players.AddLast(PlayerFactory.Create(playerId, playerId.ToString()));
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
            Debug.Log("Dealing hands");

            if (_players.Count < MinPlayers)
            {
                Debug.Log($"Tried to deal hands before {MinPlayers} joined");
                return;
            }

            if (_activePlayerNode != null)
            {
                Debug.Log("Tried to deal hands during a game");
                return;
            }

            for (var i = 0; i < StartingHandCardCount; i++)
            {
                foreach (var hand in HandManagers)
                {
                    hand.Add(DeckManager.DrawTop());
                }
            }
        }

        protected virtual void TryDrawCard()
        {
            Debug.Log("Trying to draw card");

            if (IsSelfTurn && CurrentStateId == WaitingForDrawCardStateId)
            {
                DrawCardServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        protected virtual void DrawCardServerRpc(ServerRpcParams serverRpcParams = default)
        {
            Debug.Log("Drawing card");

            var senderId = serverRpcParams.Receive.SenderClientId;

            if (senderId != _activePlayerNode.Value.Id)
            {
                Debug.Log($"Player ({senderId}) tried to draw a card out of turn");
                return;
            }

            if (CurrentStateId != WaitingForDrawCardStateId)
            {
                Debug.Log($"Player ({senderId}) tried to draw in the wrong state");
                return;
            }

            var card = DeckManager.DrawTop();
            _activePlayerNode.Value.Hand.Add(card);

            StateTriggerClientRpc(DrawCardTriggerId);
        }

        protected virtual void TryPlayCard(ICard card)
        {
            Debug.Log($"Trying to play card {card}");

            if (IsSelfTurn && CurrentStateId == WaitingForPlayCardStateId)
            {
                PlayCardServerRpc(card.Suit, card.Rank);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        protected virtual void PlayCardServerRpc(SuitEnum suit, RankEnum rank, ServerRpcParams serverRpcParams = default)
        {
            Debug.Log("Playing card");

            var senderId = serverRpcParams.Receive.SenderClientId;

            if (senderId != _activePlayerNode.Value.Id)
            {
                Debug.Log($"Player ({senderId}) tried to play a card out of turn");
                return;
            }

            if (CurrentStateId != WaitingForPlayCardStateId)
            {
                Debug.Log($"Player ({senderId}) tried to play a card when they weren't allowed to");
                return;
            }

            _activePlayerNode.Value.Hand.Remove(suit, rank);
            DiscardManager.PlaceTop(CardFactory.Create(suit, rank));

            StateTriggerClientRpc(PlayCardTriggerId);
        }

        protected virtual void EndGame()
        {
            Debug.Log("Ending game");

            _activePlayerNode = null;
        }
    }
}
