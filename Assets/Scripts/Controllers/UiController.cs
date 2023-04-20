using UnityEngine;
using UnityEngine.UI;

using InterruptingCards.Managers;

namespace InterruptingCards.Controllers
{
    public class UiController : MonoBehaviour
    {
        [SerializeField] private Button _serverButton;
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _clientButton;

        private void Awake()
        {
            _serverButton.onClick.AddListener(() => NetworkManagerDecorator.Singleton.StartServer());
            _hostButton.onClick.AddListener(() => NetworkManagerDecorator.Singleton.StartHost());
            _clientButton.onClick.AddListener(() => NetworkManagerDecorator.Singleton.StartClient());
        }
    }
}
