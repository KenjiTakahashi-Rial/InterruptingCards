using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Factories;
using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public class AbstractGameManager<S, R> : NetworkBehaviour where S : Enum where R : Enum
    {
        private const int GameStateMachineLayer = 0;
        private const int RequiredPlayersCount = 2;

        // Assign to its singleton in the concrete GameManager
        protected readonly IPlayerFactory<S, R> _playerFactory;

        private readonly int _waitingForClientsStateId = Animator.StringToHash("Base.WaitingForClients");
        private readonly int _waitingForDrawCardStateId = Animator.StringToHash("Base.InGame.WaitingForDrawCard"); // TODO: Double check that this full path is correct
        private readonly int _waitingForPlayCardStateId = Animator.StringToHash("Base.InGame.WaitingForPlayCard");
        private readonly int _startGameTriggerId = Animator.StringToHash("startGame");
        private readonly int _drawCardTriggerId = Animator.StringToHash("drawCard");
        private readonly int _playCardTriggerId = Animator.StringToHash("playCard");
        private readonly int _forceEndTurnTriggerId = Animator.StringToHash("forceEndTurn");
        private readonly int _forceEndGameTriggerId = Animator.StringToHash("forceEndGame");

        private readonly NetworkManagerDecorator _networkManager = NetworkManagerDecorator.Singleton;
        private readonly LinkedList<IPlayer<S, R>> _players = new();

        [SerializeField] private Animator _gameStateMachine;
        [SerializeField] private DeckManager<S, R> _deckManager;
        [SerializeField] private DeckManager<S, R> _discardManager;
        [SerializeField] private HandManager<S, R>[] _handManagers;

        private IPlayer<S, R> _self; // The player that is on this device
        private LinkedListNode<IPlayer<S, R>> _playerTurnNode;
        private Dictionary<IPlayer<S, R>, IHand<S, R>> _playerHands;

        public static AbstractGameManager<S, R> Singleton { get; private set; }

        private bool IsSelfTurn
        {
            get { return _playerTurnNode.Value == _self; }
        }

        private int CurrentStateId
        {
            get { return _gameStateMachine.GetCurrentAnimatorStateInfo(GameStateMachineLayer).fullPathHash ; }
        }

        public override void OnNetworkSpawn()
        {
            GetSelfServerRpc();

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

            base.OnNetworkSpawn();
        }

        public override void OnNetworkDespawn()
        {
            _networkManager.OnClientConnectedCallback -= TryAddPlayer;
            _networkManager.OnClientDisconnectCallback -= TryRemovePlayer;

            _deckManager.OnDeckClicked -= TryDrawCard;

            foreach (var handManager in _handManagers)
            {
                handManager.OnCardClicked -= TryPlayCard;
            }

            base.OnNetworkDespawn();
        }

        public void StartGame()
        {
            _deckManager.ResetDeck();
            AssignHands();
            _playerTurnNode = _players.First;
            StateTrigger(_startGameTriggerId);
        }

        public override void OnDestroy()
        {
            Singleton = null;
        }

        private void Start()
        {
            Singleton = this;
        }

        private void StateTrigger(int triggerId)
        {
            _gameStateMachine.SetTrigger(triggerId);
        }

        private void ShiftTurn(int times = 1)
        {
            for (var i = 0; i < times; i++)
            {
                _playerTurnNode = _playerTurnNode.Next ?? _players.First;
            }
        }

        private void EndGame()
        {
            _playerTurnNode = null;
        }

        [ServerRpc]
        private void GetSelfServerRpc(ServerRpcParams serverRpcParams = default)
        {
            var clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { serverRpcParams.Receive.SenderClientId }
                }
            };

            GetSelfClientRpc(clientRpcParams);
        }

        [ClientRpc]
        private void GetSelfClientRpc(ClientRpcParams clientRpcParams)
        {
            var selfClientId = clientRpcParams.Send.TargetClientIds.First();
            _self = _players.First(p => p.Id == selfClientId);
        }

        private void AssignHands()
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

        private void TryAddPlayer(ulong clientId)
        {
            if (IsServer && CurrentStateId == _waitingForClientsStateId && _players.Count < RequiredPlayersCount)
            {
                AddPlayerServerRpc(clientId);
            }
        }

        [ServerRpc]
        private void AddPlayerServerRpc(ulong clientId)
        {
            if (CurrentStateId != _waitingForClientsStateId)
            {
                Debug.LogWarning($"Did not add player ({clientId}): game already started");
                return;
            }

            if (_players.Count == RequiredPlayersCount)
            {
                Debug.LogWarning($"Did not add player ({clientId}): lobby is full");
                return;
            }

            _players.AddLast(_playerFactory.CreatePlayer(clientId, clientId.ToString()));

            if (_players.Count == RequiredPlayersCount)
            {
                StartGame();
            }
        }

        private void TryRemovePlayer(ulong clientId)
        {
            if (!IsServer)
            {
                Debug.LogWarning($"Client attempted to remove player ({clientId})");
                return;
            }

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

        private void TryDrawCard()
        {
            if (IsSelfTurn && CurrentStateId == _waitingForDrawCardStateId)
            {
                DrawCardServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void DrawCardServerRpc(ServerRpcParams serverRpsParams = default)
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

        private void TryPlayCard(ICard<S, R> card)
        {
            if (IsSelfTurn && CurrentStateId == _waitingForPlayCardStateId)
            {
                PlayCardServerRpc(card.Suit, card.Rank);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void PlayCardServerRpc(S suit, R rank, ServerRpcParams serverRpsParams = default)
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

            var card = _playerHands[_playerTurnNode.Value].Remove(suit, rank);
            _discardManager.PlaceTop(card);

            StateTrigger(_playCardTriggerId);
        }
    }
}
