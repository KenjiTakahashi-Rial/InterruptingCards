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

        [SerializeField] private StateMachineManager _stateMachineManager;
        [SerializeField] private int _minPlayers;
        [SerializeField] private int _maxPlayers;

        private int _activePlayerIndex;

        public Player ActivePlayer => _players.Count == 0 ? null : _players[_activePlayerIndex];

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
            _activePlayerIndex = 0;
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

        public void ShiftTurn(int shifts)
        {
            Debug.Log($"Shifting turn {shifts} times");

            for (var i = 0; i < shifts; i++)
            {
                _activePlayerIndex = ++_activePlayerIndex == _players.Count ? 0 : _activePlayerIndex;
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

            if (_stateMachineManager.CurrentState != StateMachine.WaitingForClientsState)
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
                _stateMachineManager.SetTrigger(StateMachine.WaitForReadyTrigger);
                SetPlayersClientRpc(_lobby.ToArray());
            }
        }

        [ServerRpc]
        private void RemovePlayerServerRpc(ulong clientId)
        {
            Debug.Log($"Removing player {clientId}");

            if (_stateMachineManager.CurrentState != StateMachine.WaitingForClientsState)
            {
                if (clientId == ActivePlayer.Id)
                {
                    _stateMachineManager.SetTrigger(StateMachine.ForceEndTurnTrigger);
                }

                Debug.LogWarning($"Player {clientId} removed while game is playing");
            }

            // TODO: Continue game if enough players left
            _stateMachineManager.SetTrigger(StateMachine.ForceEndGameTrigger);
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
                _stateMachineManager.SetTrigger(StateMachine.AllReadyTrigger);
            }
        }
    }
}
