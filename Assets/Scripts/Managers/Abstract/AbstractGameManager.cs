using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Factories;
using InterruptingCards.Models;

namespace InterruptingCards.Managers.GameManagers
{
    public abstract class AbstractGameManager<S, R> : IGameManager<S, R> where S : Enum where R : Enum
    {
        protected const int GameStateMachineLayer = 0;

        protected readonly int _waitingForClientsStateId = Animator.StringToHash("Base.WaitingForClients");
        protected readonly int _waitingForDrawCardStateId = Animator.StringToHash("Base.InGame.WaitingForDrawCard"); // TODO: Double check that this full path is correct
        protected readonly int _waitingForPlayCardStateId = Animator.StringToHash("Base.InGame.WaitingForPlayCard");
        protected readonly int _startGameTriggerId = Animator.StringToHash("startGame");
        protected readonly int _drawCardTriggerId = Animator.StringToHash("drawCard");
        protected readonly int _playCardTriggerId = Animator.StringToHash("playCard");
        protected readonly int _forceEndTurnTriggerId = Animator.StringToHash("forceEndTurn");
        protected readonly int _forceEndGameTriggerId = Animator.StringToHash("forceEndGame");

        protected readonly NetworkManagerDecorator _networkManager = NetworkManagerDecorator.Singleton;
        protected readonly IPlayerFactory<S, R> _playerFactory;
        protected readonly ICardFactory<S, R> _cardFactory;
        protected readonly LinkedList<IPlayer<S, R>> _players = new();

        [SerializeField] protected Animator _gameStateMachine;
        [SerializeField] protected IDeckManager<S, R> _deckManager;
        [SerializeField] protected IDeckManager<S, R> _discardManager;
        [SerializeField] protected IHandManager<S, R>[] _handManagers;

        protected IPlayer<S, R> _self; // The player that is on this device
        protected LinkedListNode<IPlayer<S, R>> _playerTurnNode;
        protected Dictionary<IPlayer<S, R>, IHand<S, R>> _playerHands;

        public static IGameManager<S, R> Singleton { get; private set; }

        protected abstract IGameManagerNetworkDependency<S, R> NetworkDependency { get; }

        protected abstract int MinPlayers { get; }

        protected abstract int MaxPlayers { get; }

        protected bool IsSelfTurn
        {
            get { return _playerTurnNode.Value == _self; }
        }

        protected int CurrentStateId
        {
            get { return _gameStateMachine.GetCurrentAnimatorStateInfo(GameStateMachineLayer).fullPathHash; }
        }

        protected AbstractGameManager(IPlayerFactory<S, R> playerFactory, ICardFactory<S, R> cardFactory)
        {
            _playerFactory = playerFactory;
            _cardFactory = cardFactory;
            Singleton = this;
        }

        public void OnNetworkSpawn()
        {
            _networkManager.OnClientConnectedCallback -= TryAddPlayer;
            _networkManager.OnClientConnectedCallback += TryAddPlayer;
            _networkManager.OnClientDisconnectCallback -= TryRemovePlayer;
            _networkManager.OnClientDisconnectCallback += TryRemovePlayer;

            _deckManager.OnDeckClicked -= TryDrawCard;
            _deckManager.OnDeckClicked += TryDrawCard;

            foreach (var handManager in _handManagers)
            {
                handManager.OnCardClicked -= TryPlayCard;
                handManager.OnCardClicked += TryPlayCard;
            }
        }

        public void OnNetworkDespawn()
        {
            _networkManager.OnClientConnectedCallback -= TryAddPlayer;
            _networkManager.OnClientDisconnectCallback -= TryRemovePlayer;

            _deckManager.OnDeckClicked -= TryDrawCard;

            foreach (var handManager in _handManagers)
            {
                handManager.OnCardClicked -= TryPlayCard;
            }
        }

        protected void StateTrigger(int triggerId)
        {
            _gameStateMachine.SetTrigger(triggerId);
        }

        public virtual void StartGame()
        {
            NetworkDependency.GetSelfServerRpc();
            _deckManager.ResetDeck();
            AssignHands();
            TryDealHands();
            _playerTurnNode = _players.First;
            StateTrigger(_startGameTriggerId);
        }

        protected void ShiftTurn(int times = 1)
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
            if (CurrentStateId == _waitingForClientsStateId && _players.Count < MinPlayers)
            {
                NetworkDependency.AddPlayerServerRpc(clientId);
            }
        }

        public void AddPlayer(ulong clientId)
        {
            if (CurrentStateId != _waitingForClientsStateId)
            {
                Debug.LogWarning($"Did not add player ({clientId}): game already started");
                return;
            }

            if (_players.Count == MaxPlayers)
            {
                Debug.LogWarning($"Did not add player ({clientId}): lobby is full");
                return;
            }

            _players.AddLast(_playerFactory.CreatePlayer(clientId, clientId.ToString()));
        }

        protected virtual void TryRemovePlayer(ulong clientId)
        {
            if (!_networkManager.ConnectedClients.ContainsKey(clientId))
            {
                NetworkDependency.RemovePlayerServerRpc(clientId);
            }

            // TODO: Remove their hand
        }

        public void RemovePlayer(ulong clientId)
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

            if (_players.Count <= 1)
            {
                StateTrigger(_forceEndGameTriggerId);
            }
        }

        public void GetSelf(ServerRpcParams serverRpcParams)
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

        public void AssignSelf(ClientRpcParams clientRpcParams)
        {
            var selfClientId = clientRpcParams.Send.TargetClientIds.First();
            _self = _players.First(p => p.Id == selfClientId);
        }

        protected virtual void AssignHands()
        {
            if (_players.Count > _handManagers.Count())
            {
                throw new TooManyPlayersException();
            }

            _playerHands = new();
            var i = 0;
            foreach (var player in _players)
            {
                var hand = _handManagers[i++];
                player.Hand = hand;
                _playerHands[player] = hand;
            }
        }

        protected virtual void TryDealHands()
        {
            if (_players.Count < MinPlayers)
            {
                NetworkDependency.DealHandsServerRpc();
            }
        }

        public abstract void DealHands();

        protected virtual void TryDrawCard()
        {
            if (IsSelfTurn && CurrentStateId == _waitingForDrawCardStateId)
            {
                NetworkDependency.DrawCardServerRpc();
            }
        }

        public void DrawCard(ServerRpcParams serverRpcParams)
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

            var card = _deckManager.DrawTop();
            _playerHands[_playerTurnNode.Value].Add(card);

            StateTrigger(_drawCardTriggerId);
        }

        protected virtual void TryPlayCard(ICard<S, R> card)
        {
            if (IsSelfTurn && CurrentStateId == _waitingForPlayCardStateId)
            {
                NetworkDependency.PlayCardServerRpc(card.Suit, card.Rank);
            }
        }

        public void PlayCard(S suit, R rank, ServerRpcParams serverRpcParams)
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
            _discardManager.PlaceTop(_cardFactory.CreateCard(suit, rank));

            StateTrigger(_playCardTriggerId);
        }
    }
}
