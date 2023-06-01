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

        private readonly NetworkVariable<int> _cardId = new(CardConfig.GetCardId(s_defaultSuit, s_defaultRank));
        private readonly NetworkVariable<bool> _isFaceUp = new(s_defaultIsFaceUp);
        private readonly NetworkVariable<bool> _isActivated = new(s_defaultIsActivated);

        [SerializeField] private static CardSuit s_defaultSuit = CardSuit.Invalid;
        [SerializeField] private static CardRank s_defaultRank = CardRank.Invalid;
        [SerializeField] private static bool s_defaultIsFaceUp = true;
        [SerializeField] private static bool s_defaultIsActivated = false;

        [SerializeField] private TextMeshPro _cardText;
        [SerializeField] private SpriteRenderer _cardSprite;

        private Vector3 _originalScale;
        private Quaternion _originalRotation;
        private Quaternion _activatedRotation;

        public Action OnClicked { get; set; }
        public Action OnActivated { get; set; }

        public int CardId
        {
            get => _cardId.Value;
            set => _cardId.Value = value;
        }

        public bool IsFaceUp
        {
            get => _isFaceUp.Value;
            set => _isFaceUp.Value = value;
        }

        public bool IsActivated
        {
            get => _isActivated.Value;
            set => _isActivated.Value = value;
        }

        public override void OnNetworkSpawn()
        {
            _cardId.OnValueChanged += HandleCardIdChanged;
            _isFaceUp.OnValueChanged += HandleFaceUpChanged;
            _isActivated.OnValueChanged += HandleActivatedChanged;
        }

        public override void OnNetworkDespawn()
        {
            _cardId.OnValueChanged -= HandleCardIdChanged;
            _isFaceUp.OnValueChanged -= HandleFaceUpChanged;
            _isActivated.OnValueChanged -= HandleActivatedChanged;
        }

        private void HandleCardIdChanged(int oldValue, int newValue)
        {
            var oldCard = _cardConfig.GetCardString(oldValue);
            var newCard = _cardConfig.GetCardString(newValue);
            Debug.Log($"Card changed ({oldCard} -> {newCard})");

            _cardSprite.enabled = newValue != CardConfig.InvalidId;
            _cardText.enabled = newValue != CardConfig.InvalidId && IsFaceUp;
            _cardText.SetText(_cardConfig.GetCardString(newValue)); // TODO: Change
        }

        private void HandleFaceUpChanged(bool oldValue, bool newValue)
        {
            var before = oldValue ? "face-up" : "face-down";
            var after = newValue ? "face-up" : "face-down";
            Debug.Log($"Card changed ({before} -> {after})");

            _cardText.enabled = (_cardId.Value != CardConfig.InvalidId) && newValue;
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
                Debug.Log($"{_cardConfig.GetCardString(_cardId.Value)} clicked");
            }

            OnClicked?.Invoke();
        }
    }
}