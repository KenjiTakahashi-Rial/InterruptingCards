using System;

using InterruptingCards.Models;

using TMPro;

using UnityEngine;

namespace InterruptingCards
{
    public class CardBehaviour<S, R> : MonoBehaviour where S : Enum where R : Enum
    {
        [SerializeField] private TextMeshPro _cardText;

        private bool _isFaceUp = true;
        private ICard<S, R> _card;

        public event Action OnCardClicked;

        public bool IsFaceUp
        {
            get { return _isFaceUp; }
            set
            {
                _isFaceUp = value;
                Refresh();
            }
        }

        public ICard<S, R> Card
        {
            get { return _card; }
            set
            {
                _card = value;
                Refresh();
            }
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

            _cardText.SetText(_card.ToString());
            _cardText.enabled = true;
        }

        private void OnMouseDown()
        {
            OnCardClicked.Invoke();
        }
    }
}