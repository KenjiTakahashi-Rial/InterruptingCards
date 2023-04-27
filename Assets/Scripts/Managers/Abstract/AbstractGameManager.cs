using System.Collections.Generic;
using System.Linq;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Factories;
using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public abstract class AbstractGameManager : NetworkBehaviour, IGameManager
    {
        /******************************************************************************************\
         * Constants                                                                              *
        \******************************************************************************************/

        protected const int GameStateMachineLayer = 0;

        /******************************************************************************************\
         * Fields                                                                                 *
        \******************************************************************************************/

        // TODO: Double check that this full path format is correct
        protected readonly int _waitingForClientsStateId = Animator.StringToHash("Base.WaitingForClients");
        protected readonly int _waitingForDrawCardStateId = Animator.StringToHash("Base.InGame.WaitingForDrawCard");
        protected readonly int _waitingForPlayCardStateId = Animator.StringToHash("Base.InGame.WaitingForPlayCard");
        protected readonly int _startGameTriggerId = Animator.StringToHash("startGame");
        protected readonly int _drawCardTriggerId = Animator.StringToHash("drawCard");
        protected readonly int _playCardTriggerId = Animator.StringToHash("playCard");
        protected readonly int _forceEndTurnTriggerId = Animator.StringToHash("forceEndTurn");
        protected readonly int _forceEndGameTriggerId = Animator.StringToHash("forceEndGame");

        protected readonly NetworkList<ulong> _playerTurnOrder = new();
        protected readonly Dictionary<ulong, IPlayer> _playerMap = new();

        [SerializeField] protected Animator _gameStateMachine;

        protected int _playerTurnIndex = -1;
        protected IPlayer _self; // The player that is on this device

        /******************************************************************************************\
         * Properties                                                                             *
        \******************************************************************************************/

        public static IGameManager Singleton { get; protected set; }

        protected virtual NetworkManager Network => NetworkManager.Singleton;

        protected abstract IPlayerFactory PlayerFactory { get; }

        protected abstract ICardFactory CardFactory { get; }

        protected abstract IHandFactory HandFactory { get; }

        protected abstract IDeckManager DeckManager { get; }

        protected abstract IDeckManager DiscardManager { get; }

        protected abstract IHandManager[] HandManagers { get; }

        protected abstract int MinPlayers { get; }

        protected abstract int MaxPlayers { get; }

        protected abstract int StartingHandCardCount { get; }

        protected virtual IPlayer ActivePlayer =>
            _playerTurnIndex < 0 ? null : _playerMap[_playerTurnOrder[_playerTurnIndex]];

        protected virtual bool IsSelfTurn => ActivePlayer.Id == _self.Id;

        protected virtual int CurrentStateId =>
            _gameStateMachine.GetCurrentAnimatorStateInfo(GameStateMachineLayer).fullPathHash;

        /******************************************************************************************\
         * Public Methods                                                                         *
        \******************************************************************************************/

        public override void OnNetworkSpawn()
        {
            GetSelfServerRpc();

            // OnClientConnectedCallback for host happens before network spawns
            if (IsHost)
            {
                TryAddPlayer(_self.Id);
            }

            Network.OnClientConnectedCallback -= TryAddPlayer;
            Network.OnClientConnectedCallback += TryAddPlayer;
            Network.OnClientDisconnectCallback -= TryRemovePlayer;
            Network.OnClientDisconnectCallback += TryRemovePlayer;

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
            Network.OnClientConnectedCallback -= TryAddPlayer;
            Network.OnClientDisconnectCallback -= TryRemovePlayer;

            DeckManager.OnDeckClicked -= TryDrawCard;

            foreach (var handManager in HandManagers)
            {
                handManager.OnCardClicked -= TryPlayCard;
            }
        }

        public virtual void StartGame()
        {
            Debug.Log("Starting Game");
            DeckManager.ResetDeck();
            AssignHands();
            TryDealHands();
            _playerTurnIndex = 0;
        }

        /******************************************************************************************\
         * Protected Methods                                                                      *
        \******************************************************************************************/

        protected virtual void Awake()
        {
            Singleton = this;
        }

        protected virtual new void OnDestroy()
        {
            Singleton = null;
            base.OnDestroy();
        }

        protected virtual void StateTrigger(int triggerId)
        {
            _gameStateMachine.SetTrigger(triggerId);
        }

        protected virtual void ShiftTurn(int times = 1)
        {
            for (var i = 0; i < times; i++)
            {
                _playerTurnIndex = ++_playerTurnIndex == _playerTurnOrder.Count ? 0 : _playerTurnIndex;
            }
        }

        [ServerRpc]
        public virtual void GetSelfServerRpc(ServerRpcParams serverRpcParams = default)
        {
            var clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { serverRpcParams.Receive.SenderClientId }
                }
            };

            AssignSelfClientRpc(clientRpcParams);
        }

        [ClientRpc]
        protected virtual void AssignSelfClientRpc(ClientRpcParams clientRpcParams)
        {
            var selfClientId = clientRpcParams.Send.TargetClientIds.First();
            _self = _playerMap[selfClientId];
        }

        protected virtual void AssignHands()
        {
            if (_playerMap.Count > HandManagers.Count())
            {
                throw new TooManyPlayersException();
            }

            var i = 0;
            foreach (var player in _playerMap.Values)
            {
                HandManagers[i].Hand = HandFactory.Create(new List<ICard>());
                var hand = HandManagers[i++];
                player.Hand = hand;
            }
        }

        protected virtual void TryAddPlayer(ulong clientId)
        {
            if (CurrentStateId == _waitingForClientsStateId && _playerMap.Count < MaxPlayers)
            {
                AddPlayerServerRpc(clientId);
            }
        }

        [ServerRpc]
        protected virtual void AddPlayerServerRpc(ulong clientId)
        {
            if (CurrentStateId != _waitingForClientsStateId)
            {
                Debug.LogWarning($"Did not add player ({clientId}): game already started");
                return;
            }

            if (_playerMap.Count >= MaxPlayers)
            {
                Debug.LogWarning($"Did not add player ({clientId}): lobby is full");
                return;
            }

            _playerMap[clientId] = PlayerFactory.Create(clientId, clientId.ToString());
            _playerTurnOrder.Add(clientId);
            
            // TODO: Change this to a button or something
            if (_playerMap.Count == MaxPlayers)
            {
                StateTrigger(_startGameTriggerId);
            }
        }

        protected virtual void TryRemovePlayer(ulong clientId)
        {
            if (!Network.ConnectedClients.ContainsKey(clientId))
            {
                RemovePlayerServerRpc(clientId);
            }

            // TODO: Remove their hand
        }

        [ServerRpc]
        protected virtual void RemovePlayerServerRpc(ulong clientId)
        {
            if (CurrentStateId != _waitingForClientsStateId)
            {
                if (clientId == ActivePlayer.Id)
                {
                    StateTrigger(_forceEndTurnTriggerId);

                }
                Debug.LogWarning($"Player ({clientId}) removed while game is playing");
            }

            _playerMap.Remove(clientId);

            if (_playerMap.Count < MinPlayers)
            {
                StateTrigger(_forceEndGameTriggerId);
            }
        }

        protected virtual void TryDealHands()
        {
            if (_playerMap.Count >= MinPlayers && _playerTurnIndex < 0)
            {
                DealHandsServerRpc();
            }
        }

        [ServerRpc]
        protected virtual void DealHandsServerRpc()
        {
            if (_playerMap.Count < MinPlayers)
            {
                Debug.Log($"Tried to deal hands before {MinPlayers} joined");
                return;
            }

            if (_playerTurnIndex >= 0)
            {
                Debug.Log($"Tried to deal hands during a game");
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
            if (IsSelfTurn && CurrentStateId == _waitingForDrawCardStateId)
            {
                DrawCardServerRpc();
            }
        }

        [ServerRpc]
        protected virtual void DrawCardServerRpc(ServerRpcParams serverRpcParams = default)
        {
            var senderId = serverRpcParams.Receive.SenderClientId;

            if (senderId != ActivePlayer.Id)
            {
                Debug.Log($"Player ({senderId}) tried to draw a card out of turn");
                return;
            }

            if (CurrentStateId == _waitingForDrawCardStateId)
            {
                Debug.Log($"Player ({senderId}) tried to draw when they weren't allowed to");
                return;
            }

            var card = DeckManager.DrawTop();
            ActivePlayer.Hand.Add(card);

            StateTrigger(_drawCardTriggerId);
        }

        protected virtual void TryPlayCard(ICard card)
        {
            if (IsSelfTurn && CurrentStateId == _waitingForPlayCardStateId)
            {
                PlayCardServerRpc(card.Suit, card.Rank);
            }
        }

        [ServerRpc]
        public virtual void PlayCardServerRpc(SuitEnum suit, RankEnum rank, ServerRpcParams serverRpcParams = default)
        {
            var senderId = serverRpcParams.Receive.SenderClientId;

            if (senderId != ActivePlayer.Id)
            {
                Debug.Log($"Player ({senderId}) tried to play a card out of turn");
                return;
            }

            if (CurrentStateId != _waitingForPlayCardStateId)
            {
                Debug.Log($"Player ({senderId}) tried to play a card when they weren't allowed to");
                return;
            }

            ActivePlayer.Hand.Remove(suit, rank);
            DiscardManager.PlaceTop(CardFactory.Create(suit, rank));

            StateTrigger(_playCardTriggerId);
        }

        protected virtual void EndGame()
        {
            _playerTurnIndex = -1;
        }
    }
}
