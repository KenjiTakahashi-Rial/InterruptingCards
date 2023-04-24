using System;

using UnityEngine;

using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public abstract class AbstractDeckManager<S, R> : MonoBehaviour, IDeckManager<S, R> where S : Enum where R : Enum
    {
        [SerializeField] private CardBehaviour<S, R> _topCard;

        private bool _isFaceUp = false;
        private IDeck<S, R> _deck;

        public event Action OnDeckClicked;

        public bool IsFaceUp
        {
            get { return _isFaceUp; }
            set
            {
                _isFaceUp = value;
                Refresh();
            }
        }

        public IDeck<S, R> Deck
        {
            get { return _deck; }
            set
            {
                _deck = value;
                Refresh();
            }
        }

        public int Count()
        {
            return _deck.Count();
        }

        public void Shuffle()
        {
            _deck.Shuffle();
            Refresh();
        }
        public void PlaceTop(ICard<S, R> card)
        {
            _deck.PlaceTop(card);
            Refresh();
        }

        public void PlaceBottom(ICard<S, R> card)
        {
            _deck.PlaceBottom(card);
            Refresh();
        }

        public void InsertRandom(ICard<S, R> card)
        {
            _deck.InsertRandom(card);
            Refresh();
        }

        public ICard<S, R> PeekTop()
        {
            return _deck.PeekTop();
        }

        public ICard<S, R> DrawTop()
        {
            var card = _deck.DrawTop();
            Refresh();
            return card;
        }

        public ICard<S, R> DrawBottom()
        {
            var card = _deck.DrawBottom();
            Refresh();
            return card;
        }

        public ICard<S, R> Remove(S suit, R rank)
        {
            var card = _deck.Remove(suit, rank);
            Refresh();
            return card;
        }

        public abstract void ResetDeck();

        private void OnEnable()
        {
            _topCard.OnCardClicked -= InvokeOnDeckClicked;
            _topCard.OnCardClicked += InvokeOnDeckClicked;
        }

        private void OnDisable()
        {
            _topCard.OnCardClicked -= InvokeOnDeckClicked;
        }

        private void Refresh()
        {
            _topCard.IsFaceUp = IsFaceUp;
            _topCard.Card = _deck.Count() == 0 ? null : PeekTop();
        }

        private void InvokeOnDeckClicked()
        {
            OnDeckClicked.Invoke();
        }
    }
}
