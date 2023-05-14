using System;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Models;
using InterruptingCards.Config;
using InterruptingCards.Factories;

namespace InterruptingCards.Behaviours
{
    public class InterruptingCardBehaviour : BasicCardBehaviour, IActiveCardBehaviour<InterruptingCard>
    {
        private const float ActivatedAngle = 90;

        private readonly NetworkVariable<bool> _isActivated = new(true);

        private new InterruptingCard _card;
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

        public new InterruptingCard Card
        {
            get => _card;
            set
            {
                _card = value;
                base.Card = value;
            }
        }

        protected new ICardFactory<InterruptingCard> CardFactory => InterruptingCardFactory.Singleton;

        public override void OnNetworkSpawn()
        {
            _isActivated.OnValueChanged -= HandleActivatedChanged;
            _isActivated.OnValueChanged += HandleActivatedChanged;

// Assign to self to set NetworkVariable and refresh
#pragma warning disable S1656 // Variables should not be self-assigned
            Card = Card;
#pragma warning restore S1656 // Variables should not be self-assigned

            base.OnNetworkDespawn();
        }

        public override void OnNetworkDespawn()
        {
            _isActivated.OnValueChanged -= HandleActivatedChanged;

            base.OnNetworkDespawn();
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

        protected override void Start()
        {
            // Duplicated in other card behaviours since NetworkBehaviour doesn't work with generics :(
            // Must happen after awake so factory can be loaded by game manager
            if (_startingSuit != CardSuit.Invalid && _startingRank != CardRank.Invalid)
            {
                var cardId = CardConfig.GetCardId(_startingSuit, _startingRank);
                Card = CardFactory.Create(cardId);
            }

            base.Start();
        }

        protected override void Refresh()
        {
            base.Refresh();

            transform.rotation = IsActivated ? _activatedRotation : _originalRotation;
        }
    }
}