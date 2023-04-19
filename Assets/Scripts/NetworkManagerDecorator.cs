using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace InterruptingCards
{
    public class NetworkManagerDecorator : NetworkBehaviour
    {
        [SerializeField] private NetworkManager _networkManager;

        public static NetworkManagerDecorator Singleton { get; private set; }

        public GameObject PlayerPrefab { get; private set; }

        public IReadOnlyDictionary<ulong, NetworkClient> ConnectedClients
        {
            get { return _networkManager.ConnectedClients; }
        }

        public bool StartServer()
        {
            return _networkManager.StartServer();
        }

        public bool StartClient()
        {
            return _networkManager.StartClient();
        }

        public bool StartHost()
        {
            var success = _networkManager.StartHost();
            if (success)
            {
                // NetworkManager invokes OnClientConnected before OnServerStarted
                SpawnPlayer(_networkManager.LocalClientId);
            }
            return success;
        }

        public override void OnDestroy()
        {
            Singleton = null;
            base.OnDestroy();
        }

        private void OnEnable()
        {
            Singleton = this;

            // Remove it so it can be spawned by the decorator
            PlayerPrefab = _networkManager.NetworkConfig.PlayerPrefab;
            _networkManager.NetworkConfig.PlayerPrefab = null;

            void HandleOnServerStarted()
            {
                _networkManager.OnClientConnectedCallback -= SpawnPlayer;
                _networkManager.OnClientConnectedCallback += SpawnPlayer;
            }

            _networkManager.OnServerStarted -= HandleOnServerStarted;
            _networkManager.OnServerStarted += HandleOnServerStarted;
        }

        private void SpawnPlayer(ulong clientId)
        {
            var playerObj = ObjectManager.Singleton.InstantiatePlayer();
            var networkObj = playerObj.GetComponent<NetworkObject>();
            networkObj.SpawnAsPlayerObject(clientId, destroyWithScene: true);
        }
    }
}
