using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public class PlayerManager : NetworkBehaviour
    {
        private readonly List<ulong> _lobby = new();
        private readonly HashSet<ulong> _notReadyPlayers = new();
        private readonly List<Player> _players = new();
        private readonly NetworkVariable<int> _activePlayerIndex = new();
        private readonly Dictionary<int, NetworkVariable<int>.OnValueChangedDelegate> _onActivePlayerChangedHandlers = new();

        [SerializeField] private StateMachineManager _gameStateMachineManager;
        [SerializeField] private int _minPlayers;
        [SerializeField] private int _maxPlayers;

        public Player ActivePlayer => _players.Count == 0 ? null : _players[_activePlayerIndex.Value];

        public event Action<Player> OnActivePlayerChanged
        {
            add
            {
                void IndexToPlayer(int _, int i) { value(_players[i]); }
                _onActivePlayerChangedHandlers[value.GetHashCode()] = IndexToPlayer;
                _activePlayerIndex.OnValueChanged += IndexToPlayer;
            }

// Needs to be a delegate. Action<int, int> gives an error
#pragma warning disable S3172 // Delegates should not be subtracted
            remove => _activePlayerIndex.OnValueChanged -= _onActivePlayerChangedHandlers[value.GetHashCode()];
#pragma warning restore S3172 // Delegates should not be subtracted
        }

        public ulong SelfId { get; private set; }

        private LogManager Log => LogManager.Singleton;

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

        public ulong GetNextId(ulong id)
        {
            var i = _players.FindIndex(p => p.Id == id);
            return _players[++i == _players.Count ? 0 : i].Id;
        }

        public void AssignHands(HandManager[] hands)
        {
            if (_players.Count > hands.Length)
            {
                throw new TooManyPlayersException();
            }

            Log.Info("Assigning hands");

            var i = 0;
            foreach (var player in _players)
            {
                player.Hand = hands[i++];
            }
        }

        public void ShiftTurn(int shifts = 1)
        {
            Log.Info($"Shifting turn {shifts} times");

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

            Log.Info($"Getting self {clientId}");

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
            Log.Info($"Assigning self {selfId}");
            SelfId = selfId;
        }

        [ServerRpc]
        private void AddPlayerServerRpc(ulong clientId)
        {
            Log.Info($"Adding player {clientId}");

            if (_gameStateMachineManager.CurrentState != StateMachine.WaitingForClients)
            {
                Log.Warn($"Cannnot add player {clientId} when game already started");
                return;
            }

            if (_lobby.Count >= _maxPlayers)
            {
                Log.Warn($"Cannnot add player {clientId} when lobby is full");
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
            Log.Info($"Removing player {clientId}");

            if (_gameStateMachineManager.CurrentState != StateMachine.WaitingForClients)
            {
                if (clientId == ActivePlayer.Id)
                {
                    _gameStateMachineManager.SetTrigger(StateMachine.ForceEndTurn);
                }

                Log.Warn($"Player {clientId} removed while game is playing");
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
