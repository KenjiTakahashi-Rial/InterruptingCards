using System;

using TMPro;
using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Models;

namespace InterruptingCards.Behaviours
{
    public class CardBehaviour : NetworkBehaviour
    {
        private const float ActivatedAngle = -90;

        private readonly CardConfig _cardConfig = CardConfig.Singleton;
        private readonly CardFactory _cardFactory = CardFactory.Singleton;

        private readonly NetworkVariable<int> _cardId = new(CardConfig.InvalidId);
        private readonly NetworkVariable<bool> _isFaceUp = new(true);
        private readonly NetworkVariable<bool> _isActivated = new(false);

        [SerializeField] private TextMeshPro _cardText;
        [SerializeField] private SpriteRenderer _cardSprite;

        private Card _card;
        private bool _offlineIsFaceUp = true;
        private bool _offlineIsActivated;

        private Vector3 _originalScale;
        private Quaternion _originalRotation;
        private Quaternion _activatedRotation;

        public event Action OnClicked;
        public event Action OnCardChanged;
        public event Action OnActivated;

        public Card Card
        {
            get => _card;
            set
            {
                if (IsNetworking)
                {
                    _cardId.Value = value == null ? CardConfig.InvalidId : value.Id;
                }
                else
                {
                    _card = value;
                }
            }
        }

        public bool IsFaceUp
        {
            get => IsNetworking ? _isFaceUp.Value : _offlineIsFaceUp;
            set
            {
                if (IsNetworking)
                {
                    _isFaceUp.Value = value;
                }
                else
                {
                    _offlineIsFaceUp = value;
                }
            }
        }

        public bool IsActivated
        {
            get => NetworkManager ? _isActivated.Value : _offlineIsActivated;
            set
            {
                if (IsNetworking)
                {
                    _isActivated.Value = value;
                }
                else
                {
                    _offlineIsActivated = value;
                }
            }
        }

        private bool IsNetworking => NetworkManager != null && NetworkManager.IsListening;

        public override void OnNetworkSpawn()
        {
            _cardId.OnValueChanged -= HandleCardIdChanged;
            _isFaceUp.OnValueChanged -= HandleFaceUpChanged;
            _isActivated.OnValueChanged -= HandleActivatedChanged;

            _cardId.OnValueChanged += HandleCardIdChanged;
            _isFaceUp.OnValueChanged += HandleFaceUpChanged;
            _isActivated.OnValueChanged += HandleActivatedChanged;

            if (IsServer)
            {
                _cardId.Value = _card == null ? CardConfig.InvalidId : _card.Id;
            }
        }

        public override void OnNetworkDespawn()
        {
            _cardId.OnValueChanged -= HandleCardIdChanged;
            _isFaceUp.OnValueChanged -= HandleFaceUpChanged;
            _isActivated.OnValueChanged -= HandleActivatedChanged;
        }

        public void UnsubscribeAllOnClicked()
        {
            OnClicked = null;
        }

        public void UnsubscribeAllOnCardChanged()
        {
            OnCardChanged = null;
        }

        public void UnsubscribeAllOnActivated()
        {
            OnActivated = null;
        }

        private void HandleCardIdChanged(int oldValue, int newValue)
        {
            var oldCard = _cardConfig.GetCardString(oldValue);
            var newCard = _cardConfig.GetCardString(newValue);
            Debug.Log($"Card changed ({oldCard} -> {newCard})");

            _card = newValue == CardConfig.InvalidId ? null : _cardFactory.Create(newValue);
            _cardSprite.enabled = _card != null;
            _cardText.enabled = _card != null;
            _cardText.SetText(_card?.ToString()); // TODO: Change

            if (OnCardChanged == null)
            {
                Debug.Log("OnCardChanged has no subscribers");
            }

            OnCardChanged?.Invoke();
        }

        private void HandleFaceUpChanged(bool oldValue, bool newValue)
        {
            var before = oldValue ? "face-up" : "face-down";
            var after = newValue ? "face-up" : "face-down";
            Debug.Log($"Card changed ({before} -> {after})");

            _cardText.enabled = _card != null && newValue;
        }

        private void HandleActivatedChanged(bool oldValue, bool newValue)
        {
            var before = oldValue ? "activated" : "not activated";
            var after = newValue ? "activated" : "not activated";
            Debug.Log($"Active card changed ({before} -> {after})");

            transform.rotation = IsActivated ? _activatedRotation : _originalRotation;

            if (OnActivated == null)
            {
                Debug.Log("OnActivated has no subscribers");
            }

            OnActivated?.Invoke();
        }

        private void Awake()
        {
            _originalScale = transform.localScale;
            _originalRotation = transform.rotation;
            var angles = _originalRotation.eulerAngles;
            _activatedRotation = Quaternion.Euler(angles.x, angles.y, angles.z + ActivatedAngle);
            transform.rotation = _originalRotation;

            _cardSprite.enabled = false;
            _cardText.enabled = false;
        }

        private void Update()
        {
            // TODO: NetworkManager spawns the objects at wrong scale. Figure out why and find a better solution
            if (transform.localScale != _originalScale)
            {
                transform.localScale = _originalScale;
            }
        }

        private void OnMouseDown()
        {
            if (OnClicked == null)
            {
                Debug.Log("OnClicked has no subscribers");
            }
            else
            {
                Debug.Log($"{_card} clicked");
            }

            OnClicked?.Invoke();
        }
    }
}