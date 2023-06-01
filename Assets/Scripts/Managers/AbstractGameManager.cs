using System.Collections.Generic;

using TMPro;
using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Models;
using System.Linq;

namespace InterruptingCards.Managers
{
    public abstract class AbstractGameManager : NetworkBehaviour
    {
        /******************************************************************************************\
         * Fields                                                                                 *
        \******************************************************************************************/
        
        protected const int GameStateMachineLayer = 0;

        protected readonly StateMachineConfig _stateMachineConfig = StateMachineConfig.Singleton;
        protected readonly CardConfig _cardConfig = CardConfig.Singleton;

        // Server-only
        protected readonly List<ulong> _lobby = new();
        protected readonly HashSet<ulong> _notReadyPlayers = new();
        
        protected readonly List<Player> _players = new();

        [SerializeField] protected CardPack _cardPack;
        [SerializeField] protected Animator _gameStateMachine;
        [SerializeField] protected DeckManager _deckManager;
        [SerializeField] protected DeckManager _discardManager;
        [SerializeField] protected HandManager[] _handManagers;
        [SerializeField] protected TextMeshPro _tempInfoText;

        protected int _activePlayerIndex;
        protected ulong _selfId; // The player that is on this device

        /******************************************************************************************\
         * Properties                                                                             *
        \******************************************************************************************/

        public static AbstractGameManager Singleton { get; protected set; }

        public string CurrentStateName => _stateMachineConfig.GetName(CurrentStateId);

        protected virtual int MinPlayers => 2;

        protected virtual int MaxPlayers => 2;

        protected virtual int StartingHandCardCount => 4;

        protected virtual int CurrentStateId =>
            _gameStateMachine.GetCurrentAnimatorStateInfo(GameStateMachineLayer).fullPathHash;

        protected virtual Player ActivePlayer => _players.Count == 0 ? null : _players[_activePlayerIndex];

        /******************************************************************************************\
         * Unity Methods                                                                          *
        \******************************************************************************************/

        public virtual void Awake()
        {
            Debug.Log("Waking game manager");

            _cardConfig.Load(_cardPack);
            Singleton = this;
        }

        public virtual void Update()
        {
            if (ActivePlayer == null || _tempInfoText == null)
            {
                return;
            }

            _tempInfoText.SetText($"{ActivePlayer.Id}\n{CurrentStateName}");
        }

        public override void OnNetworkSpawn()
        {
            Debug.Log("Network spawned");

            GetSelfServerRpc();

            if (IsServer)
            {
                NetworkManager.OnClientConnectedCallback += AddPlayerServerRpc;
                NetworkManager.OnClientDisconnectCallback += RemovePlayerServerRpc;
            }

            _deckManager.OnClicked += TryDrawCard;

            for (var i = 0; i < _handManagers.Length; i++)
            {
                var j = i;
                _handManagers[i].OnCardClicked += (int k) => TryPlayCard(j, k);
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

            _deckManager.OnClicked -= TryDrawCard;

            foreach (var handManager in _handManagers)
            {
                handManager.OnCardClicked = null;
            }

            if (CurrentStateId != _stateMachineConfig.GetId(StateMachine.WaitingForClientsState))
            {
                _gameStateMachine.SetTrigger(_stateMachineConfig.GetId(StateMachine.ForceEndGameTrigger));
            }
        }

        public override void OnDestroy()
        {
            Debug.Log("Destroying game manager");

            Singleton = null;
            _players.Clear();
            base.OnDestroy();
        }

        /******************************************************************************************\
         * State Machine Methods                                                                  *
        \******************************************************************************************/

        public virtual void HandleInitializeGame()
        {
            Debug.Log("Initializing Game");

            AssignHands();
            
            if (IsServer)
            {
                _deckManager.Initialize();
                _deckManager.Shuffle();
                _deckManager.IsFaceUp = false;
                _discardManager.Clear();
                DealHandsServerRpc();
            }

            _activePlayerIndex = 0;

            StateTriggerClientRpc(StateMachine.StartGameTrigger);
        }

        public abstract void HandleStartTurn();

        public virtual void HandleEndTurn(int shifts = 1)
        {
            Debug.Log($"Shifting turn {shifts} times");

            for (var i = 0; i < shifts; i++)
            {
                _activePlayerIndex = ++_activePlayerIndex == _players.Count ? 0 : _activePlayerIndex;
            }
        }

        public virtual void HandleEndGame()
        {
            Debug.Log("Ending game");

            _players.Clear();
            _tempInfoText.SetText("Start the game");

            if (IsServer)
            {
                _deckManager.Clear();
                _discardManager.Clear();

                foreach (var handManager in _handManagers)
                {
                    handManager.Clear();
                }
            }
        }

        /******************************************************************************************\
         * Game Flow Methods                                                                      *
        \******************************************************************************************/

        [ClientRpc]
        protected virtual void StateTriggerClientRpc(StateMachine trigger, ClientRpcParams clientRpcParams = default)
        {
            var triggerId = _stateMachineConfig.GetId(trigger);
            Debug.Log($"Triggering {trigger}");
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

            Debug.Log("Assigning hands");

            var i = 0;
            foreach (var player in _players)
            {
                player.Hand = _handManagers[i++];
            }
        }

        [ServerRpc]
        protected virtual void AddPlayerServerRpc(ulong clientId)
        {
            Debug.Log($"Adding player {clientId}");

            if (CurrentStateId != _stateMachineConfig.GetId(StateMachine.WaitingForClientsState))
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
                StateTriggerClientRpc(StateMachine.WaitForReadyTrigger);
                SetPlayersClientRpc(_lobby.ToArray());
            }
        }

        [ServerRpc]
        protected virtual void RemovePlayerServerRpc(ulong clientId)
        {
            Debug.Log($"Removing player {clientId}");

            if (CurrentStateId != _stateMachineConfig.GetId(StateMachine.WaitingForClientsState))
            {
                if (clientId == ActivePlayer.Id)
                {
                    StateTriggerClientRpc(StateMachine.ForceEndTurnTrigger);
                }

                Debug.LogWarning($"Player {clientId} removed while game is playing");
            }

            // TODO: Continue game if enough players left
            StateTriggerClientRpc(StateMachine.ForceEndGameTrigger);
            _lobby.Remove(clientId);
        }

        [ClientRpc]
        protected virtual void SetPlayersClientRpc(ulong[] playerIds, ClientRpcParams clientRpcParams = default)
        {
            foreach (var playerId in playerIds)
            {
               _players.Add(new Player(playerId, playerId.ToString()));
            }

            PlayerReadyServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        protected virtual void PlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
        {
            _notReadyPlayers.Remove(serverRpcParams.Receive.SenderClientId);
            
            if (_notReadyPlayers.Count == 0)
            {
                StateTriggerClientRpc(StateMachine.AllReadyTrigger);
            }
        }

        [ServerRpc]
        protected virtual void DealHandsServerRpc()
        {
            if (_players.Count < MinPlayers)
            {
                Debug.LogWarning($"Cannot deal hands before {MinPlayers} joined");
                return;
            }

            if (CurrentStateId != _stateMachineConfig.GetId(StateMachine.InitializingGameState))
            {
                Debug.LogWarning("Cannot deal hands outside of game initialization state");
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
            if (id != ActivePlayer.Id)
            {
                Debug.Log($"Player {id} cannot draw a card unless it is their turn");
                return false;
            }

            if (CurrentStateId != _stateMachineConfig.GetId(StateMachine.WaitingForDrawCardState))
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
            ActivePlayer.Hand.Add(card);

            StateTriggerClientRpc(StateMachine.DrawCardTrigger);
        }

        protected virtual bool CanPlayCard(ulong id, int handManagerIndex)
        {
            if (id != ActivePlayer.Id)
            {
                Debug.Log($"Player {id} cannot play a card unless it is their turn or they are interrupting");
                return false;
            }

            if (_handManagers[handManagerIndex] != ActivePlayer.Hand)
            {
                Debug.Log($"Player {id} can only play cards from their own hand");
                return false;
            }

            if (CurrentStateId != _stateMachineConfig.GetId(StateMachine.WaitingForPlayCardState))
            {
                Debug.Log($"Player {id} cannot play a card in the wrong state");
                return false;
            }

            return true;
        }

        protected virtual void TryPlayCard(int handManagerIndex, int cardIndex)
        {
            var cardId = _handManagers[handManagerIndex][cardIndex];

            Debug.Log($"Trying to play card {_cardConfig.GetCardString(cardId)}");

            if (CanPlayCard(_selfId, handManagerIndex))
            {
                PlayCardServerRpc(handManagerIndex, cardIndex);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        protected virtual void PlayCardServerRpc(int handManagerIndex, int cardIndex, ServerRpcParams serverRpcParams = default)
        {
            var senderId = serverRpcParams.Receive.SenderClientId;

            if (!CanPlayCard(senderId, handManagerIndex))
            {
                return;
            }

            Debug.Log("Playing card");

            var cardId = _handManagers[handManagerIndex].RemoveAt(cardIndex);
            _discardManager.PlaceTop(cardId);

            StateTriggerClientRpc(StateMachine.PlayCardTrigger);
        }
    }
}
