using Unity.Netcode;
using UnityEngine;

namespace InterruptingCards
{
    public class ObjectManager : MonoBehaviour
    {
        [SerializeField] private Transform[] _playerSpawnPoints;

        public static ObjectManager Singleton { get; private set; }

        private void OnEnable()
        {
            Singleton = this;
        }

        private void OnDestroy()
        {
            Singleton = null;
        }

        public GameObject InstantiatePlayer()
        {
            if (_playerSpawnPoints.Length == 0)
            {
                throw new ObjectManagerException("Failed to spawn player: Player spawn points is empty");
            }

            var clientCount = NetworkManager.Singleton.ConnectedClients.Count;
            var i = (clientCount - 1) % _playerSpawnPoints.Length;
            var obj = Instantiate(
                NetworkManagerDecorator.Singleton.PlayerPrefab, 
                _playerSpawnPoints[i].position,
                Quaternion.identity
            );
            obj.SetActive(true);
            return obj;
        }
    }
}
