using System;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Models;

namespace InterruptingCards.Behaviours
{
    public class InterruptingCardBehaviour : BasicCardBehaviour, IActiveCardBehaviour<BasicCard>
    {
        private const float ActivatedAngle = 90;

        private readonly NetworkVariable<bool> _isActivated = new(true);

        private new IActiveCard _card;
        private Quaternion _originalRotation;
        private Quaternion _activatedRotation;

        public event Action OnActivated;

        public bool IsActivated
        {
            get => _isActivated.Value;
            set
            {
                _isActivated.Value = value;
                Refresh();
            }
        }

        public new IActiveCard Card
        {
            get => _card;
            set
            {
                _card = value;

                base.Card = (BasicCard)value;
            }
        }

        public void UnsubscribeAllOnActivated()
        {
            OnActivated = null;
        }

        protected void HandleActivatedChanged(bool oldValue, bool newValue)
        {
            var before = oldValue ? "activated" : "not activated";
            var after = newValue ? "activated" : "not activated";
            Debug.Log($"Active card changed ({before} -> {after})");

            if (OnActivated == null)
            {
                Debug.Log("OnActivated has no subscribers");
            }

            OnActivated?.Invoke();
            Refresh();
        }

        protected override void Awake()
        {
            base.Awake();

            _originalRotation = transform.rotation;
            transform.Rotate(Vector3.up, ActivatedAngle);
            _activatedRotation = transform.rotation;
            transform.rotation = _originalRotation;
        }

        protected override void Refresh()
        {
            base.Refresh();

            transform.rotation = IsActivated ? _activatedRotation : _originalRotation;
        }
    }
}