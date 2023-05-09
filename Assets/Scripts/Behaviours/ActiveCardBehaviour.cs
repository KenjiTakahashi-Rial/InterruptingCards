using System;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Models;

namespace InterruptingCards.Behaviours
{
    public class ActiveCardBehaviour : BasicCardBehaviour, IActiveEffect
    {
        private const float ActivateAngle = 90;

        private readonly NetworkVariable<ActiveEffect> _effect = new(ActiveEffect.Invalid);
        private readonly NetworkVariable<bool> _isActivated = new(true);

        [SerializeField] private ActiveEffect _startingEffect;

        private Quaternion _originalRotation;
        private Quaternion _activatedRotation;

        public ActiveEffect Effect
        {
            get => _effect.Value;
            set => _effect.Value = value;
        }

        public bool IsActivated
        {
            get => _isActivated.Value;
            set
            {
                _isActivated.Value = value;
                Refresh();
            }
        }

        public new ICard Card
        {
            get => base.Card;
            set
            {
                if (value is not IActiveEffect)
                {
                    throw new ArgumentException("Must use a card with an active effect");
                }

                base.Card = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            _originalRotation = transform.rotation;
            transform.Rotate(Vector3.up, ActivateAngle);
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