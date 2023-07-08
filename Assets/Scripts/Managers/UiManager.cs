using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace InterruptingCards.Managers
{
    public class UiManager : MonoBehaviour
    {
#pragma warning disable RCS1169 // Make field read-only.
        [SerializeField] private GameObject _networkButtonsParent;
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _clientButton;

        [SerializeField] private GameObject _gameplayButtonsParent;
        [SerializeField] private Button _purchaseButton;
        [SerializeField] private Button _attackButton;
        [SerializeField] private Button _endTurnButton;
        [SerializeField] private Button _passPriorityButton;
#pragma warning restore RCS1169 // Make field read-only.

        public void Awake()
        {
            _hostButton.onClick.AddListener(() => NetworkManager.Singleton.StartHost());
            _clientButton.onClick.AddListener(() => NetworkManager.Singleton.StartClient());

            _purchaseButton.onClick.AddListener(() => GameManager.Singleton.TryDeclarePurchase());
            _attackButton.onClick.AddListener(() => GameManager.Singleton.TryDeclareAttack());
            _endTurnButton.onClick.AddListener(() => GameManager.Singleton.TryDeclareEndTurn());

            _passPriorityButton.onClick.AddListener(() => GameManager.Singleton.TryPassPriority());
        }

        public void SetNetworkButtonsHidden(bool val)
        {
            _networkButtonsParent.SetActive(!val);
        }

        public void SetGameplayButtonsHidden(bool val)
        {
            _gameplayButtonsParent.SetActive(!val);
        }
    }
}
