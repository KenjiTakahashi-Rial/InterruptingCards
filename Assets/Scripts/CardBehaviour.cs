using System;

using InterruptingCards.Models;

using TMPro;

using Unity.Netcode;
using UnityEngine;

namespace InterruptingCards
{
    public class CardBehaviour<S, R> : NetworkBehaviour where S : Enum where R : Enum
    {
        [SerializeField] private TextMeshPro _cardText;

        private readonly NetworkVariable<bool> _isFaceUp = new(true);
        private readonly NetworkVariable<ICard<S, R>> _card = new();

        public event Action OnCardClicked;

        public bool IsFaceUp
        {
            get { return _isFaceUp.Value; }
            set
            {
                _isFaceUp.Value = value;
                Refresh();
            }
        }

        public ICard<S, R> Card
        {
            get { return _card.Value; }
            set
            {
                _card.Value = value;
                Refresh();
            }
        }

        public override void OnNetworkSpawn()
        {
            _card.OnValueChanged -= Refresh;
            _card.OnValueChanged += Refresh;
            _isFaceUp.OnValueChanged -= Refresh;
            _isFaceUp.OnValueChanged += Refresh;
        }

        public override void OnNetworkDespawn()
        {
            _card.OnValueChanged -= Refresh;
            _isFaceUp.OnValueChanged -= Refresh;
        }

        public void UnsubscribeAllOnCardClicked()
        {
            OnCardClicked = null;
        }

        private void Refresh()
        {
            if (Card == null)
            {
                _cardText.gameObject.SetActive(false);
                return;
            }

            if (!IsFaceUp)
            {
                _cardText.enabled = false;
                return;
            }

            _cardText.SetText(_card.Value.ToString()); // TODO: Change  this
            _cardText.enabled = true;
        }

        // The parameters are necessary for use in NetworkVariable.OnValueChanged
#pragma warning disable S1172 // Unused method parameters should be removed
        private void Refresh<T>(T _, T v)
#pragma warning restore S1172 // Unused method parameters should be removed
        {
            Refresh();
        }

        private void OnMouseDown()
        {
            OnCardClicked.Invoke();
        }
    }
}