using System;

using TMPro;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Models;

namespace InterruptingCards.Behaviours
{
    public class PlayingCardBehaviour : NetworkBehaviour, ICardBehaviour
    {
        private readonly NetworkVariable<bool> _isFaceUp = new(true);
        private readonly NetworkVariable<PlayingCard> _card = new(null);

        [SerializeField] private TextMeshPro _cardText;
        [SerializeField] private SpriteRenderer _cardSprite;

        public event Action OnCardClicked;

        public bool IsFaceUp
        {
            get => _isFaceUp.Value;
            set
            {
                _isFaceUp.Value = value;
                Refresh();
            }
        }

        public ICard Card
        {
            get => _card.Value;
            set
            {
                _card.Value = (PlayingCard)value;
                Refresh();
            }
        }

        public override void OnNetworkSpawn()
        {
            _card.OnValueChanged -= Refresh;
            _isFaceUp.OnValueChanged -= Refresh;

            _card.OnValueChanged += Refresh;
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
            if (_card.Value == null)
            {
                _cardSprite.enabled = false;
                _cardText.enabled = false;
            }
            else if (!IsFaceUp)
            {
                _cardSprite.enabled = true;
                _cardText.enabled = false;
            }
            else
            {
                _cardText.SetText(_card.Value.ToString()); // TODO: Change
                _cardSprite.enabled = true;
                _cardText.enabled = true;
            }
        }

        private void Refresh<T>(T _, T v)
        {
            Refresh();
        }

        private void OnMouseDown()
        {
            Debug.Log("Mouse down on card");
            OnCardClicked.Invoke();
        }
    }
}