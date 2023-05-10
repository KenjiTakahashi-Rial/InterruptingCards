using System;
using System.Collections.Generic;

namespace InterruptingCards.Models
{
    public class BasicDeck : IDeck
    {
        protected readonly Random _random = new();
        protected IList<ICard> _cards;

        public virtual int Count => _cards.Count;

        protected virtual IFactory Factory => BasicFactory.Singleton;

        protected virtual int TopIndex { get => _cards.Count - 1;}

        protected virtual int BottomIndex { get; } = 0;

        internal BasicDeck(IList<ICard> cards)
        {
            _cards = cards;
        }

        public virtual void Shuffle()
        {
            CheckEmpty();

            for (var i = 0; i < _cards.Count - 1; i++)
            {
                var j = _random.Next(0, _cards.Count);
                (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
            }
        }

        public virtual void PlaceTop(ICard card)
        {
            _cards.Add(card);
        }

        public virtual void PlaceBottom(ICard card)
        {
            _cards.Insert(0, card);
        }

        public virtual void InsertRandom(ICard card)
        {
            var i = _random.Next(0, _cards.Count + 1);
            _cards.Insert(i, card);
        }

        public virtual ICard PeekTop()
        {
            CheckEmpty();
            return Factory.CreateCard(_cards[TopIndex].Id);
        }

        public virtual ICard DrawTop()
        {
            return PopAt(TopIndex);
        }

        public virtual ICard DrawBottom()
        {
            return PopAt(BottomIndex);
        }

        public virtual ICard Remove(int cardId)
        {
            return Utilities.Remove(_cards, cardId);
        }

        protected virtual void CheckEmpty()
        {
            if (_cards.Count == 0)
            {
                throw new CardCollectionEmptyException();
            }
        }

        protected virtual ICard PopAt(int i)
        {
            CheckEmpty();
            var card = _cards[i];
            _cards.RemoveAt(i);
            return card;
        }
    }
}