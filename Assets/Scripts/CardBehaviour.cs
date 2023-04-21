using System;

using InterruptingCards.Models.Abstract;

using TMPro;

using UnityEngine;

namespace InterruptingCards
{
    public class CardBehaviour<S, R> : MonoBehaviour where S : Enum where R : Enum
    {
        [SerializeField] private TextMeshPro _cardText;

        private bool _isFaceUp = true;
        private ICard<S, R> _card;

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

        public void Refresh()
        {
            if (IsFaceUp)
            {
                _cardText.SetText(_card.ToString());
                _cardText.enabled = true;
            }
            else
            {
                _cardText.enabled = false;
            }
        }
    }
}