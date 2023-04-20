using System.Collections.Generic;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Models.Abstract;
using InterruptingCards.Models.PlayingCards;

namespace InterruptingCards.Managers
{
    public class GameManager : NetworkBehaviour, IGame
    {
        private const int GameStateLayer = 0;

        private readonly int _playingStateId = Animator.StringToHash("Game.Playing");
        private readonly int _playingParamId = Animator.StringToHash("playing");

        private readonly NetworkManagerDecorator _networkManager = NetworkManagerDecorator.Singleton;
        private readonly List<Player> _players = new();

        [SerializeField] private Animator _gameStateMachine;

        private Deck<Suit, Rank> _deck;

        private bool IsPlaying { get { return _gameStateMachine.GetCurrentAnimatorStateInfo(GameStateLayer).fullPathHash == _playingStateId; } }

        public override void OnNetworkSpawn()
        {
            _networkManager.OnClientConnectedCallback -= AddPlayer;
            _networkManager.OnClientConnectedCallback += AddPlayer;

            _networkManager.OnClientDisconnectCallback -= RemovePlayer;
            _networkManager.OnClientDisconnectCallback += RemovePlayer;
        }
        
        public void EndTurn()
        {
            throw new System.NotImplementedException();
        }

        public void Play()
        {
            _gameStateMachine.SetBool(_playingParamId, true);
        }

        private void AddPlayer(ulong clientId)
        {
            if (IsPlaying)
            {
                Debug.LogWarning($"Attempted to add player ({clientId}) while game is playing");
                return;
            }
            _players.Add(new Player(clientId.ToString(), new Hand()));
        }

        private void RemovePlayer(ulong clientId)
        {
            if (!IsServer)
            {
                Debug.LogWarning($"Client attempted to remove player ({clientId})");
                return;
            }

            if (IsPlaying)
            {
                Debug.LogWarning($"Player ({clientId}) removed while game is playing");
                return;
            }

            _players.RemoveAll(p => p.Name == clientId.ToString());
        }
    }
}
