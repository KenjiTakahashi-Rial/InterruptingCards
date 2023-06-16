using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Behaviours;
using InterruptingCards.Config;

namespace InterruptingCards.Managers
{
    public class PlayerManager : NetworkBehaviour
    {
        private readonly HashSet<ulong> _notReadyPlayers = new();
        private readonly List<PlayerBehaviour> _players = new();
        private readonly NetworkVariable<int> _activePlayerIndex = new();
        private readonly Dictionary<int, NetworkVariable<int>.OnValueChangedDelegate> _onActivePlayerChangedHandlers = new();

        [SerializeField] private int _minPlayers;
        [SerializeField] private int _maxPlayers;

        public PlayerBehaviour ActivePlayer => _players.Count == 0 ? null : _players[_activePlayerIndex.Value];

        private GameManager Game => GameManager.Singleton;

        private StateMachineManager GameStateMachineManager => Game.StateMachineManager;

        public event Action<PlayerBehaviour> OnActivePlayerChanged
        {
            add
            {
                void IndexToPlayer(int _, int i) { value(_players[i]); }
                _onActivePlayerChangedHandlers[value.GetHashCode()] = IndexToPlayer;
                _activePlayerIndex.OnValueChanged += IndexToPlayer;
            }

            remove => _activePlayerIndex.OnValueChanged -= _onActivePlayerChangedHandlers[value.GetHashCode()];
        }

        // TODO: This is temporary
        public List<PlayerBehaviour> TempPlayers => _players;

        private LogManager Log => LogManager.Singleton;

        public PlayerBehaviour this[ulong id] => _players.Single(p => p.Id == id);

        public override void OnNetworkSpawn()
        {
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

        public void Initialize(uint startingMoney)
        {
            _activePlayerIndex.Value = 0;

            foreach (var player in _players)
            {
                player.Money = startingMoney;
            }
        }

        public void AssignCharacters(CardBehaviour[] characterCards)
        {
            if (_players.Count > characterCards.Length)
            {
                throw new TooManyPlayersException();
            }

            for (var i = 0; i < _players.Count; i++)
            {
                var character = characterCards[i];
                _players[i].CharacterCard = character;
            }
        }

        public void AssignHands(HandBehaviour[] hands)
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

        public ulong GetNextId(ulong id)
        {
            var i = _players.FindIndex(p => p.Id == id);
            return _players[++i == _players.Count ? 0 : i].Id;
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

        [ServerRpc]
        private void AddPlayerServerRpc(ulong clientId)
        {
            // TODO: Look into using Unity Network Lobby Manager instead of managing it myself
            Log.Info($"Adding player {clientId}");

            if (GameStateMachineManager.CurrentState != StateMachine.WaitingForClients)
            {
                Log.Warn($"Cannnot add player {clientId} when game already started");
                return;
            }

            var clients = NetworkManager.ConnectedClientsList;
            if (clients.Count > _maxPlayers)
            {
                Log.Warn($"Found {clients.Count} clients, but max is {_maxPlayers}");
                return;
            }

            if (clients.Count >= _maxPlayers)
            {
                var networkObjIds = new ulong[_maxPlayers];
                for (var i = 0; i < _maxPlayers; i++)
                {
                    var client = clients[i];
                    var player = client.PlayerObject.gameObject.GetComponent<PlayerBehaviour>();
                    player.Id = client.ClientId;
                    player.Name = player.Id.ToString();
                    networkObjIds[i] = client.PlayerObject.NetworkObjectId;
                }

                _notReadyPlayers.UnionWith(NetworkManager.ConnectedClientsIds);
                GameStateMachineManager.SetTrigger(StateMachine.WaitForReady);
                SetPlayersClientRpc(networkObjIds);
            }
        }

        [ServerRpc]
        private void RemovePlayerServerRpc(ulong clientId)
        {
            Log.Info($"Removing player {clientId}");

            if (GameStateMachineManager.CurrentState != StateMachine.WaitingForClients)
            {
                if (clientId == ActivePlayer.Id)
                {
                    GameStateMachineManager.SetTrigger(StateMachine.ForceEndTurn);
                }

                Log.Warn($"Player {clientId} removed while game is playing");
            }

            // TODO: Continue game if enough players left
            GameStateMachineManager.SetTrigger(StateMachine.ForceEndGame);
            _players.RemoveAll(p => p.Id == clientId);
        }

        [ClientRpc]
        private void SetPlayersClientRpc(ulong[] playerNetworkObjIds)
        {
            foreach (var networkObjId in playerNetworkObjIds)
            {
                var playerNetworkObj = NetworkManager.SpawnManager.SpawnedObjects[networkObjId];
                var player = playerNetworkObj.gameObject.GetComponent<PlayerBehaviour>();
                _players.Add(player);
            }

            PlayerReadyServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void PlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
        {
            _notReadyPlayers.Remove(serverRpcParams.Receive.SenderClientId);
            
            if (_notReadyPlayers.Count == 0)
            {
                GameStateMachineManager.SetTrigger(StateMachine.AllReady);
            }
        }
    }
}
