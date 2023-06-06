using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace InterruptingCards.Managers
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _clientButton;

        [SerializeField] private Button _purchaseButton;
        [SerializeField] private Button _attackButton;
        [SerializeField] private Button _lootPlayButton;
        [SerializeField] private Button _abilityButton;
        [SerializeField] private Button _endTurnButton;

        private void Awake()
        {
            _hostButton.onClick.AddListener(() => NetworkManager.Singleton.StartHost());
            _clientButton.onClick.AddListener(() => NetworkManager.Singleton.StartClient());

            _purchaseButton.onClick.AddListener(() => GameManager.Singleton.BeginDeclarePurchase());
            _attackButton.onClick.AddListener(() => GameManager.Singleton.BeginDeclareAttack());
            _lootPlayButton.onClick.AddListener(() => GameManager.Singleton.BeginPlayLoot());
            _abilityButton.onClick.AddListener(() => GameManager.Singleton.BeginActivateAbility());
            _endTurnButton.onClick.AddListener(() => GameManager.Singleton.BeginDeclareEndTurn());
        }
    }
}
