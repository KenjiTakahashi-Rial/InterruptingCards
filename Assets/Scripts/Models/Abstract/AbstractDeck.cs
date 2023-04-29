using System;
using System.Collections.Generic;

using InterruptingCards.Factories;

namespace InterruptingCards.Models
{
    public abstract class AbstractDeck : IDeck
    {
        protected readonly Random _random = new();
        protected IList<ICard> _cards;

        protected abstract ICardFactory CardFactory { get; }

        protected virtual int TopIndex { get => _cards.Count - 1;}

        protected virtual int BottomIndex { get; } = 0;

        protected AbstractDeck(IList<ICard> cards)
        {
            _cards = cards;
        }

        public abstract object Clone();

        public virtual int Count()
        {
            return _cards.Count;
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
            return CardFactory.Clone(_cards[TopIndex]);
        }

        public virtual ICard DrawTop()
        {
            return PopAt(TopIndex);
        }

        public virtual ICard DrawBottom()
        {
            return PopAt(BottomIndex);
        }

        public virtual ICard Remove(SuitEnum suit, RankEnum rank)
        {
            return Utilities.Remove(_cards, suit, rank);
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