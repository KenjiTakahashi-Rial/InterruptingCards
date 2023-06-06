using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Models;
using InterruptingCards.Utilities;

namespace InterruptingCards.Managers
{
    public class PlayerManager : NetworkBehaviour
    {
        private readonly List<ulong> _lobby = new();
        private readonly HashSet<ulong> _notReadyPlayers = new();
        private readonly List<Player> _players = new();
        private readonly Observable<int> _activePlayerIndex = new();
        private readonly Dictionary<int, Action<int>> _onActivePlayerChangedHandlers = new();

        [SerializeField] private StateMachineManager _gameStateMachineManager;
        [SerializeField] private int _minPlayers;
        [SerializeField] private int _maxPlayers;

        public Player ActivePlayer => _players.Count == 0 ? null : _players[_activePlayerIndex.Value];

        public event Action<Player> OnActivePlayerChanged
        {
            add
            {
                void IndexToPlayer(int i) { value(_players[i]); }
                _onActivePlayerChangedHandlers[value.GetHashCode()] = IndexToPlayer;
                _activePlayerIndex.OnChanged += IndexToPlayer;
            }
            remove => _activePlayerIndex.OnChanged -= _onActivePlayerChangedHandlers[value.GetHashCode()];
        }

        public ulong SelfId { get; private set; }

        public Player this[ulong id] => _players.Single(p => p.Id == id);

        public override void OnNetworkSpawn()
        {
            GetSelfServerRpc();

            if (IsServer)
            {
                NetworkManager.OnClientConnectedCallback += AddPlayerServerRpc;
                NetworkManager.OnClientDisconnectCallback += RemovePlayerServerRpc;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                Clear();
                NetworkManager.OnClientConnectedCallback -= AddPlayerServerRpc;
                NetworkManager.OnClientDisconnectCallback -= RemovePlayerServerRpc;
            }
        }

        public void Initialize()
        {
            _activePlayerIndex.Value = 0;
        }

        public Player GetNext(ulong id)
        {
            var i = _players.FindIndex(p => p.Id == id);
            return _players[++i == _players.Count ? 0 : i];
        }

        public void AssignHands(HandManager[] hands)
        {
            if (_players.Count > hands.Length)
            {
                throw new TooManyPlayersException();
            }

            Debug.Log("Assigning hands");

            var i = 0;
            foreach (var player in _players)
            {
                player.Hand = hands[i++];
            }
        }

        public void ShiftTurn(int shifts = 1)
        {
            Debug.Log($"Shifting turn {shifts} times");

            for (var i = 0; i < shifts; i++)
            {
                _activePlayerIndex.Value = (_activePlayerIndex.Value + 1) % _players.Count;
            }
        }

        public void Clear()
        {
            _players.Clear();
        }

        [ServerRpc(RequireOwnership = false)]
        private void GetSelfServerRpc(ServerRpcParams serverRpcParams = default)
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
        private void AssignSelfClientRpc(ulong selfId, ClientRpcParams _)
        {
            Debug.Log($"Assigning self {selfId}");
            SelfId = selfId;
        }

        [ServerRpc]
        private void AddPlayerServerRpc(ulong clientId)
        {
            Debug.Log($"Adding player {clientId}");

            if (_gameStateMachineManager.CurrentState != StateMachine.WaitingForClients)
            {
                Debug.LogWarning($"Cannnot add player {clientId} while game already started");
                return;
            }

            if (_lobby.Count >= _maxPlayers)
            {
                Debug.LogWarning($"Cannnot add player {clientId} while lobby is full");
                return;
            }

            _lobby.Add(clientId);

            if (_lobby.Count == _maxPlayers)
            {
                _notReadyPlayers.UnionWith(_lobby);
                _gameStateMachineManager.SetTrigger(StateMachine.WaitForReady);
                SetPlayersClientRpc(_lobby.ToArray());
            }
        }

        [ServerRpc]
        private void RemovePlayerServerRpc(ulong clientId)
        {
            Debug.Log($"Removing player {clientId}");

            if (_gameStateMachineManager.CurrentState != StateMachine.WaitingForClients)
            {
                if (clientId == ActivePlayer.Id)
                {
                    _gameStateMachineManager.SetTrigger(StateMachine.ForceEndTurn);
                }

                Debug.LogWarning($"Player {clientId} removed while game is playing");
            }

            // TODO: Continue game if enough players left
            _gameStateMachineManager.SetTrigger(StateMachine.ForceEndGame);
            _lobby.Remove(clientId);
        }

        [ClientRpc]
        private void SetPlayersClientRpc(ulong[] playerIds)
        {
            foreach (var playerId in playerIds)
            {
               _players.Add(new Player(playerId, playerId.ToString()));
            }

            PlayerReadyServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void PlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
        {
            _notReadyPlayers.Remove(serverRpcParams.Receive.SenderClientId);
            
            if (_notReadyPlayers.Count == 0)
            {
                _gameStateMachineManager.SetTrigger(StateMachine.AllReady);
            }
        }
    }
}
