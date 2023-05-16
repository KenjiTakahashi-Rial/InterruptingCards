using System;
using System.Collections.Generic;

using InterruptingCards.Utilities;

namespace InterruptingCards.Models
{
    public class Hand
    {
        private readonly IList<Card> _cards;

        public event Action OnChanged;

        internal Hand(IList<Card> cards)
        {
            _cards = cards;
        }

        public int Count => _cards.Count;

        public void Insert(int i, Card card)
        {
            _cards.Insert(i, card);
            OnChanged?.Invoke();
        }

        public void Add(Card card)
        {
            _cards.Add(card);
            OnChanged?.Invoke();
        }

        public Card Remove(int cardId)
        {
            var card = HelperMethods.Remove(_cards, cardId);
            OnChanged?.Invoke();
            return card;
        }

        public Card Get(int i)
        {
            return _cards[i];
        }

        public void Clear()
        {
            _cards.Clear();
            OnChanged?.Invoke();
        }
    }
}