using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Controllers;
using InterruptingCards.Models.Abstract;

namespace InterruptingCards.Managers
{
    public class GameManager<S, R> : NetworkBehaviour where S : Enum where R : Enum
    {
        private const int GameStateLayer = 0;
        private const int RequiredPlayersCount = 2;

        private readonly int _waitingForDrawId = Animator.StringToHash("Game.WaitingForDraw");
        private readonly int _waitingForEndTurnId = Animator.StringToHash("Game.WaitingForEndTurn");
        private readonly int _isPlayingId = Animator.StringToHash("isPlaying");
        private readonly int _drawCardId = Animator.StringToHash("drawCard");
        private readonly int _forceEndTurnId = Animator.StringToHash("forceEndTurn");
        private readonly int _forceEndGameId = Animator.StringToHash("forceEndGame");

        private readonly NetworkManagerDecorator _networkManager = NetworkManagerDecorator.Singleton;
        private readonly LinkedList<PlayerController<S, R>> _playerControllers = new();

        [SerializeField] private Animator _gameStateMachine;
        [SerializeField] private DeckController<S, R> _deckController;
        [SerializeField] private DeckController<S, R> _discardController;
        [SerializeField] private HandController<S, R>[] _handControllers;

        private LinkedListNode<PlayerController<S, R>> _playerTurnNode;
        private Dictionary<IPlayer<S, R>, IHand<S, R>> _playerHands;
        private ICard<S, R> _transferCard; // Since ServerRpc methods cannot accept a class, use this as a "parameter" to ServerRpc methods

        public static GameManager<S, R> Singleton { get; private set; }

        private bool IsPlaying {
            get { return _gameStateMachine.GetBool(_isPlayingId); }
            set { _gameStateMachine.SetBool(_isPlayingId, value); }
        }

        public override void OnNetworkSpawn()
        {
            _networkManager.OnClientConnectedCallback -= AddPlayer;
            _networkManager.OnClientConnectedCallback += AddPlayer;
            _networkManager.OnClientDisconnectCallback -= RemovePlayer;
            _networkManager.OnClientDisconnectCallback += RemovePlayer;

            _deckController.OnDeckClicked -= DrawCard;
            _deckController.OnDeckClicked += DrawCard;

            _handControllers.Select(h => {
                h.OnCardClicked -= PlayCard;
                h.OnCardClicked += PlayCard;
            });
        }

        public void Play()
        {
            _deckController.ResetDeck();
            AssignHands();
            _playerTurnNode = _playerControllers.First;
            IsPlaying = true;
        }

        public override void OnDestroy()
        {
            Singleton = null;
        }

        private void OnEnable()
        {
            Singleton = this;
        }

        private void Trigger(int triggerId)
        {
            _gameStateMachine.SetTrigger(triggerId);
        }

        private void AssignHands()
        {
            if (_playerControllers.Count > _handControllers.Count())
            {
                throw new TooManyPlayersException();
            }

            _playerHands = new();
            var i = 0;
            foreach (var player in _playerControllers)
            {
                var hand = _handControllers[i++];
                player.Hand = hand;
                _playerHands[player] = hand;
            }
        }

        private void AddPlayer(ulong clientId)
        {
            if (IsPlaying)
            {
                Debug.LogWarning($"Did not add player ({clientId}): game already started");
                return;
            }

            if (_playerControllers.Count == RequiredPlayersCount)
            {
                Debug.LogWarning($"Did not add player ({clientId}): lobby is full");
                return;
            }

            _playerControllers.AddLast(new PlayerController<S, R>(clientId, clientId.ToString()));

            if (_playerControllers.Count == RequiredPlayersCount)
            {
                Play();
            }
        }

        private void RemovePlayer(ulong clientId)
        {
            if (!IsServer)
            {
                Debug.LogWarning($"Client attempted to remove player ({clientId})");
                return;
            }

            var player = _playerControllers.FirstOrDefault(p => p.Id == clientId) ?? throw new PlayerNotFoundException();

            if (IsPlaying)
            {
                if (player == _playerTurnNode.Value)
                {
                    Trigger(_forceEndTurnId);
                    
                }
                Debug.LogWarning($"Player ({clientId}) removed while game is playing");
            }

            _playerControllers.Remove(player);

            if (_playerControllers.Count <= 1)
            {
                Trigger(_forceEndGameId);
            }
        }

        private void DrawCard()
        {
            DrawCardServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void DrawCardServerRpc(ServerRpcParams serverRpsParams = default)
        {
            var senderId = serverRpsParams.Receive.SenderClientId;
            if (senderId != _playerTurnNode.Value.Id)
            {
                Debug.Log($"Player ({senderId}) tried to draw a card out of turn.");
                return;
            }

            var card = _deckController.DrawTop();
            _playerHands[_playerTurnNode.Value].Add(card);
        }

        private void PlayCard(ICard<S, R> card)
        {
            _transferCard = card;
            PlayCardServerRpc();
            _transferCard = null;
        }

        [ServerRpc(RequireOwnership = false)]
        private void PlayCardServerRpc(ServerRpcParams serverRpsParams = default)
        {
            var senderId = serverRpsParams.Receive.SenderClientId;
            if (senderId != _playerTurnNode.Value.Id)
            {
                Debug.Log($"Player ({senderId}) tried to play a card out of turn.");
                return;
            }

            _playerHands[_playerTurnNode.Value].Remove(_transferCard.Suit, _transferCard.Rank);
            _discardController.PlaceTop(_transferCard);
        }

        private void NextTurn()
        {
            _playerTurnNode = _playerTurnNode.Next ?? _playerControllers.First;
        }

        private void EndGame()
        {
            IsPlaying = false;
            _playerTurnNode = null;
        }
    }
}
