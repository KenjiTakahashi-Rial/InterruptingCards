using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Factories;
using InterruptingCards.Models;

namespace InterruptingCards.Managers.GameManagers
{
    public abstract class AbstractGameManager : MonoBehaviour, IGameManager
    {
        protected const int GameStateMachineLayer = 0;

        // TODO: Double check that this full path format is correct
        protected readonly int _waitingForClientsStateId = Animator.StringToHash("Base.WaitingForClients");
        protected readonly int _waitingForDrawCardStateId = Animator.StringToHash("Base.InGame.WaitingForDrawCard");
        protected readonly int _waitingForPlayCardStateId = Animator.StringToHash("Base.InGame.WaitingForPlayCard");
        protected readonly int _startGameTriggerId = Animator.StringToHash("startGame");
        protected readonly int _drawCardTriggerId = Animator.StringToHash("drawCard");
        protected readonly int _playCardTriggerId = Animator.StringToHash("playCard");
        protected readonly int _forceEndTurnTriggerId = Animator.StringToHash("forceEndTurn");
        protected readonly int _forceEndGameTriggerId = Animator.StringToHash("forceEndGame");

        protected readonly LinkedList<IPlayer> _players = new();

        [SerializeField] protected Animator _gameStateMachine;

        protected IPlayer _self; // The player that is on this device
        protected LinkedListNode<IPlayer> _playerTurnNode;
        protected Dictionary<IPlayer, IHand> _playerHands;

        public static IGameManager Singleton { get; protected set; }

        protected virtual NetworkManager NetworkManager => NetworkManager.Singleton;

        protected abstract IGameManagerNetworkDependency NetworkDependency { get; }

        protected abstract IPlayerFactory PlayerFactory { get; }

        protected abstract ICardFactory CardFactory { get; }

        protected abstract IHandFactory HandFactory { get; }

        protected abstract IDeckManager DeckManager { get; }

        protected abstract IDeckManager DiscardManager { get; }

        protected abstract IHandManager[] HandManagers { get; }

        protected abstract int MinPlayers { get; }

        protected abstract int MaxPlayers { get; }

        protected abstract int StartingHandCardCount { get; }

        protected virtual bool IsSelfTurn
        {
            get => _playerTurnNode.Value == _self;
        }

        protected virtual int CurrentStateId
        {
            get => _gameStateMachine.GetCurrentAnimatorStateInfo(GameStateMachineLayer).fullPathHash;
        }

        public virtual void OnNetworkSpawn()
        {
            NetworkManager.OnClientConnectedCallback -= TryAddPlayer;
            NetworkManager.OnClientConnectedCallback += TryAddPlayer;
            NetworkManager.OnClientDisconnectCallback -= TryRemovePlayer;
            NetworkManager.OnClientDisconnectCallback += TryRemovePlayer;

            DeckManager.OnDeckClicked -= TryDrawCard;
            DeckManager.OnDeckClicked += TryDrawCard;

            foreach (var handManager in HandManagers)
            {
                handManager.OnCardClicked -= TryPlayCard;
                handManager.OnCardClicked += TryPlayCard;
            }
        }

        public virtual void OnNetworkDespawn()
        {
            NetworkManager.OnClientConnectedCallback -= TryAddPlayer;
            NetworkManager.OnClientDisconnectCallback -= TryRemovePlayer;

            DeckManager.OnDeckClicked -= TryDrawCard;

            foreach (var handManager in HandManagers)
            {
                handManager.OnCardClicked -= TryPlayCard;
            }
        }

        public virtual void AddPlayer(ulong clientId)
        {
            if (CurrentStateId != _waitingForClientsStateId)
            {
                Debug.LogWarning($"Did not add player ({clientId}): game already started");
                return;
            }

            if (_players.Count >= MaxPlayers)
            {
                Debug.LogWarning($"Did not add player ({clientId}): lobby is full");
                return;
            }

            _players.AddLast(PlayerFactory.Create(clientId, clientId.ToString()));

            if (_players.Count == MaxPlayers)
            {
                StateTrigger(_startGameTriggerId);
            }
        }

        public virtual void RemovePlayer(ulong clientId)
        {
            var player = _players.FirstOrDefault(p => p.Id == clientId) ?? throw new PlayerNotFoundException();

            if (CurrentStateId != _waitingForClientsStateId)
            {
                if (player == _playerTurnNode.Value)
                {
                    StateTrigger(_forceEndTurnTriggerId);

                }
                Debug.LogWarning($"Player ({clientId}) removed while game is playing");
            }

            _players.Remove(player);

            if (_players.Count <= MinPlayers)
            {
                StateTrigger(_forceEndGameTriggerId);
            }
        }

        public virtual void StartGame()
        {
            Debug.Log("Starting Game");
            NetworkDependency.GetSelfServerRpc();
            DeckManager.ResetDeck();
            AssignHands();
            TryDealHands();
            _playerTurnNode = _players.First;
        }

        public virtual void GetSelf(ServerRpcParams serverRpcParams)
        {
            var clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { serverRpcParams.Receive.SenderClientId }
                }
            };

            NetworkDependency.AssignSelfClientRpc(clientRpcParams);
        }

        public virtual void AssignSelf(ClientRpcParams clientRpcParams)
        {
            var selfClientId = clientRpcParams.Send.TargetClientIds.First();
            _self = _players.First(p => p.Id == selfClientId);
        }

        public virtual void DealHands()
        {
            if (_players.Count < MinPlayers)
            {
                Debug.Log($"Tried to deal hands before {MinPlayers} joined");
                return;
            }

            if (_playerTurnNode != null)
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

        public virtual void DrawCard(ServerRpcParams serverRpcParams)
        {
            var senderId = serverRpcParams.Receive.SenderClientId;

            if (senderId != _playerTurnNode.Value.Id)
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
            _playerHands[_playerTurnNode.Value].Add(card);

            StateTrigger(_drawCardTriggerId);
        }

        public virtual void PlayCard(SuitEnum suit, RankEnum rank, ServerRpcParams serverRpcParams)
        {
            var senderId = serverRpcParams.Receive.SenderClientId;

            if (senderId != _playerTurnNode.Value.Id)
            {
                Debug.Log($"Player ({senderId}) tried to play a card out of turn");
                return;
            }

            if (CurrentStateId != _waitingForPlayCardStateId)
            {
                Debug.Log($"Player ({senderId}) tried to play a card when they weren't allowed to");
                return;
            }

            _playerHands[_playerTurnNode.Value].Remove(suit, rank);
            DiscardManager.PlaceTop(CardFactory.Create(suit, rank));

            StateTrigger(_playCardTriggerId);
        }

        protected virtual void Awake()
        {
            Singleton = this;
        }

        protected virtual void OnDestroy()
        {
            Singleton = null;
        }

        protected virtual void StateTrigger(int triggerId)
        {
            _gameStateMachine.SetTrigger(triggerId);
        }

        protected virtual void ShiftTurn(int times = 1)
        {
            for (var i = 0; i < times; i++)
            {
                _playerTurnNode = _playerTurnNode.Next ?? _players.First;
            }
        }

        protected virtual void EndGame()
        {
            _playerTurnNode = null;
        }

        protected virtual void TryAddPlayer(ulong clientId)
        {
            if (CurrentStateId == _waitingForClientsStateId && _players.Count < MaxPlayers)
            {
                NetworkDependency.AddPlayerServerRpc(clientId);
            }
        }

        protected virtual void TryRemovePlayer(ulong clientId)
        {
            if (!NetworkManager.ConnectedClients.ContainsKey(clientId))
            {
                NetworkDependency.RemovePlayerServerRpc(clientId);
            }

            // TODO: Remove their hand
        }

        protected virtual void AssignHands()
        {
            if (_players.Count > HandManagers.Count())
            {
                throw new TooManyPlayersException();
            }

            _playerHands = new();
            var i = 0;
            foreach (var player in _players)
            {
                HandManagers[i].Hand = HandFactory.Create(new List<ICard>());
                var hand = HandManagers[i++];
                player.Hand = hand;
                _playerHands[player] = hand;
            }
        }

        protected virtual void TryDealHands()
        {
            if (_players.Count < MinPlayers && _playerTurnNode == null)
            {
                NetworkDependency.DealHandsServerRpc();
            }
        }

        protected virtual void TryDrawCard()
        {
            if (IsSelfTurn && CurrentStateId == _waitingForDrawCardStateId)
            {
                NetworkDependency.DrawCardServerRpc();
            }
        }

        protected virtual void TryPlayCard(ICard card)
        {
            if (IsSelfTurn && CurrentStateId == _waitingForPlayCardStateId)
            {
                NetworkDependency.PlayCardServerRpc(card.Suit, card.Rank);
            }
        }
    }
}
