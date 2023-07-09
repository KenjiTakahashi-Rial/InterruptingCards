using System;

using TMPro;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Managers;

namespace InterruptingCards.Behaviours
{
    public class CardBehaviour : NetworkBehaviour
    {
        private const int DefaultCardId = CardConfig.InvalidId;
        private const bool DefaultIsFaceUp = true;
        private const bool DefaultIsDeactivated = false;

        private const float ActivatedAngle = -90;

        private readonly CardConfig _cardConfig = CardConfig.Singleton;

        private readonly NetworkVariable<int> _cardId = new(DefaultCardId);
        private readonly NetworkVariable<bool> _isFaceUp = new(DefaultIsFaceUp);
        private readonly NetworkVariable<bool> _isDeactivated = new(DefaultIsDeactivated);

#pragma warning disable RCS1169 // Make field read-only.
        [SerializeField] private GameObject _parent;
        [SerializeField] private TextMeshProUGUI _cardText;
#pragma warning restore RCS1169 // Make field read-only.

        private Vector3 _originalScale;
        private Quaternion _originalRotation;
        private Quaternion _activatedRotation;
        private bool _isHidden;

        public Action OnClicked { get; set; }

        public Action OnDeactivated { get; set; }

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

        public bool IsDeactivated
        {
            get => _isDeactivated.Value;
            set => _isDeactivated.Value = value;
        }

        public bool IsHidden
        {
            get => _isHidden;
            set
            {
                _isHidden = value;
                _parent.SetActive(
                    !value &&
#if UNITY_EDITOR
                    (
                        !EditorApplication.isPlaying ||
#endif
                        CardId != CardConfig.InvalidId
#if UNITY_EDITOR
                    )
#endif
                );
            }
        }

        private LogManager Log => LogManager.Singleton;

        public void Awake()
        {
            _originalScale = transform.localScale;
            _originalRotation = transform.rotation;
            var angles = _originalRotation.eulerAngles;
            _activatedRotation = Quaternion.Euler(angles.x, angles.y, angles.z + ActivatedAngle);
            transform.rotation = _originalRotation;
        }

        public override void OnNetworkSpawn()
        {
            _cardId.OnValueChanged += HandleCardIdChanged;
            _isFaceUp.OnValueChanged += HandleFaceUpChanged;
            _isDeactivated.OnValueChanged += HandleActivatedChanged;
        }

        public void Update()
        {
            // TODO: NetworkManager spawns the objects at wrong scale. Figure out why and find a better solution
            if (transform.localScale != _originalScale)
            {
                transform.localScale = _originalScale;
            }
        }

        // Requires collider component
        public void OnMouseDown()
        {
            if (!_parent.activeSelf)
            {
                return;
            }

            if (OnClicked == null)
            {
                Log.Info("OnClicked has no subscribers");
            }
            else
            {
                Log.Info($"{_cardConfig.GetName(_cardId.Value)} clicked");
            }

            OnClicked?.Invoke();
        }

        public override void OnNetworkDespawn()
        {
            _cardId.OnValueChanged -= HandleCardIdChanged;
            _isFaceUp.OnValueChanged -= HandleFaceUpChanged;
            _isDeactivated.OnValueChanged -= HandleActivatedChanged;
        }

        private void HandleCardIdChanged(int oldValue, int newValue)
        {
            var oldCard = _cardConfig.GetName(oldValue);
            var newCard = _cardConfig.GetName(newValue);
            Log.Info($"Card changed ({oldCard} -> {newCard})");

            _parent.SetActive(!IsHidden && newValue != CardConfig.InvalidId);
            _cardText.SetText(_cardConfig.GetName(newValue)); // TODO: Change
        }

        private void HandleFaceUpChanged(bool oldValue, bool newValue)
        {
            var before = oldValue ? "face-up" : "face-down";
            var after = newValue ? "face-up" : "face-down";
            Log.Info($"Card changed ({before} -> {after})");

            _cardText.enabled = newValue;
        }

        private void HandleActivatedChanged(bool oldValue, bool newValue)
        {
            var before = oldValue ? "activated" : "not activated";
            var after = newValue ? "activated" : "not activated";
            Log.Info($"Active card changed ({before} -> {after})");

            transform.rotation = IsDeactivated ? _activatedRotation : _originalRotation;

            if (OnDeactivated == null)
            {
                Log.Info("OnActivated has no subscribers");
            }

            OnDeactivated?.Invoke();
        }
    }
}