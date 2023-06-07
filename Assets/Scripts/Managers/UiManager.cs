using InterruptingCards.Actions;

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

            _purchaseButton.onClick.AddListener(() => GameManager.Singleton.TryDeclarePurchase());
            _attackButton.onClick.AddListener(() => GameManager.Singleton.TryDeclareAttack());
            _lootPlayButton.onClick.AddListener(() => GameManager.Singleton.TryPlayLoot());
            _abilityButton.onClick.AddListener(() => GameManager.Singleton.TryActivateAbility());
            _endTurnButton.onClick.AddListener(() => GameManager.Singleton.BeginDeclareEndTurn());
        }
    }
}
