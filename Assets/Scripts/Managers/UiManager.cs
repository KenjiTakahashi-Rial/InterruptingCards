using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace InterruptingCards.Managers
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField] private Button _serverButton;
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _clientButton;

        private void Awake()
        {
            _serverButton.onClick.AddListener(() => NetworkManager.Singleton.StartServer());
            _hostButton.onClick.AddListener(() => NetworkManager.Singleton.StartHost());
            _clientButton.onClick.AddListener(() => NetworkManager.Singleton.StartClient());
        }
    }
}
