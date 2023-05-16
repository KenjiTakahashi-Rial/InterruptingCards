using System;
using System.Collections.Generic;

using InterruptingCards.Utilities;

namespace InterruptingCards.Models
{
    public class Deck
    {
        private readonly Random _random = new();
        private readonly CardFactory _cardFactory = CardFactory.Singleton;
        private readonly IList<Card> _cards;

        public event Action OnChanged;

        public int Count => _cards.Count;

        private int TopIndex { get => _cards.Count - 1;}

        private int BottomIndex { get; } = 0;

        internal Deck(IList<Card> cards)
        {
            _cards = cards;
        }

        public void Clear()
        {
            _cards.Clear();
            OnChanged?.Invoke();
        }

        public void Shuffle()
        {
            CheckEmpty();

            for (var i = 0; i < _cards.Count - 1; i++)
            {
                var j = _random.Next(0, _cards.Count);
                (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
            }

            OnChanged?.Invoke();
        }

        public void PlaceTop(Card card)
        {
            _cards.Add(card);
            OnChanged?.Invoke();
        }

        public void PlaceBottom(Card card)
        {
            _cards.Insert(0, card);
            OnChanged?.Invoke();
        }

        public void InsertRandom(Card card)
        {
            var i = _random.Next(0, _cards.Count + 1);
            _cards.Insert(i, card);
            OnChanged?.Invoke();
        }

        public Card PeekTop()
        {
            CheckEmpty();
            var card = _cardFactory.Create(_cards[TopIndex].Id);
            return card;
        }

        public Card DrawTop()
        {
            var card = PopAt(TopIndex);
            return card;
        }

        public Card DrawBottom()
        {
            var card = PopAt(BottomIndex);
            return card;
        }

        public Card Remove(int cardId)
        {
            var card = HelperMethods.Remove(_cards, cardId);
            OnChanged?.Invoke();
            return card;
        }

        private void CheckEmpty()
        {
            if (_cards.Count == 0)
            {
                throw new CardCollectionEmptyException();
            }
        }

        private Card PopAt(int i)
        {
            CheckEmpty();
            var card = _cards[i];
            _cards.RemoveAt(i);
            OnChanged?.Invoke();
            return card;
        }
    }
}