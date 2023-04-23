using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Factories;
using InterruptingCards.Models;

namespace InterruptingCards.Managers.GameManagers
{
    public abstract class AbstractGameManager<S, R> where S : Enum where R : Enum
    {
        protected internal const int GameStateMachineLayer = 0;

        protected internal readonly int _waitingForClientsStateId = Animator.StringToHash("Base.WaitingForClients");
        protected internal readonly int _waitingForDrawCardStateId = Animator.StringToHash("Base.InGame.WaitingForDrawCard"); // TODO: Double check that this full path is correct
        protected internal readonly int _waitingForPlayCardStateId = Animator.StringToHash("Base.InGame.WaitingForPlayCard");
        protected internal readonly int _startGameTriggerId = Animator.StringToHash("startGame");
        protected internal readonly int _drawCardTriggerId = Animator.StringToHash("drawCard");
        protected internal readonly int _playCardTriggerId = Animator.StringToHash("playCard");
        protected internal readonly int _forceEndTurnTriggerId = Animator.StringToHash("forceEndTurn");
        protected internal readonly int _forceEndGameTriggerId = Animator.StringToHash("forceEndGame");

        protected internal readonly NetworkManagerDecorator _networkManager = NetworkManagerDecorator.Singleton;
        protected internal readonly IPlayerFactory<S, R> _playerFactory;
        protected internal readonly ICardFactory<S, R> _cardFactory;
        protected internal readonly LinkedList<IPlayer<S, R>> _players = new();

        [SerializeField] protected internal Animator _gameStateMachine;
        [SerializeField] protected internal DeckManager<S, R> _deckManager;
        [SerializeField] protected internal DeckManager<S, R> _discardManager;
        [SerializeField] protected internal HandManager<S, R>[] _handManagers;

        protected internal IPlayer<S, R> _self; // The player that is on this device
        protected internal LinkedListNode<IPlayer<S, R>> _playerTurnNode;
        protected internal Dictionary<IPlayer<S, R>, IHand<S, R>> _playerHands;

        internal static AbstractGameManager<S, R> Singleton { get; private set; }

        protected internal abstract IGameManagerNetworkDependency<S, R> NetworkDependency { get; }

        protected internal abstract int MinPlayers { get; }

        protected internal abstract int MaxPlayers { get; }

        protected internal virtual bool IsSelfTurn
        {
            get { return _playerTurnNode.Value == _self; }
        }

        protected internal virtual int CurrentStateId
        {
            get { return _gameStateMachine.GetCurrentAnimatorStateInfo(GameStateMachineLayer).fullPathHash; }
        }

        protected AbstractGameManager(IPlayerFactory<S, R> playerFactory, ICardFactory<S, R> cardFactory)
        {
            _playerFactory = playerFactory;
            _cardFactory = cardFactory;
            Singleton = this;
        }

        internal virtual void OnNetworkSpawn()
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

        internal virtual void OnNetworkDespawn()
        {
            _networkManager.OnClientConnectedCallback -= TryAddPlayer;
            _networkManager.OnClientDisconnectCallback -= TryRemovePlayer;

            _deckManager.OnDeckClicked -= TryDrawCard;

            foreach (var handManager in _handManagers)
            {
                handManager.OnCardClicked -= TryPlayCard;
            }
        }

        protected virtual void StateTrigger(int triggerId)
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
            if (CurrentStateId == _waitingForClientsStateId && _players.Count < MinPlayers)
            {
                NetworkDependency.AddPlayerServerRpc(clientId);
            }
        }

        internal virtual void AddPlayer(ulong clientId)
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
                RemovePlayer(clientId);
            }

            // TODO: Remove their hand
        }

        internal virtual void RemovePlayer(ulong clientId)
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

        internal void GetSelf(ServerRpcParams serverRpcParams)
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

        internal virtual void AssignSelf(ClientRpcParams clientRpcParams)
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

        internal abstract void DealHands();

        protected virtual void TryDrawCard()
        {
            if (IsSelfTurn && CurrentStateId == _waitingForDrawCardStateId)
            {
                NetworkDependency.DrawCardServerRpc();
            }
        }

        internal virtual void DrawCard(ServerRpcParams serverRpsParams)
        {
            var senderId = serverRpsParams.Receive.SenderClientId;

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

        internal virtual void PlayCard(S suit, R rank, ServerRpcParams serverRpsParams)
        {
            var senderId = serverRpsParams.Receive.SenderClientId;

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