using System;
using System.Collections.Generic;

using InterruptingCards.Factories;

namespace InterruptingCards.Models
{
    public abstract class AbstractDeck<S, R> : IDeck<S, R> where S : Enum where R : Enum
    {
        protected readonly Random _random = new();
        protected IList<ICard<S, R>> _cards;

        protected abstract ICardFactory<S, R> CardFactory { get; }

        protected virtual int TopIndex { get => _cards.Count - 1;}

        protected virtual int BottomIndex { get; } = 0;

        protected AbstractDeck(IList<ICard<S, R>> cards)
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

        public virtual void PlaceTop(ICard<S, R> card)
        {
            _cards.Add(card);
        }

        public virtual void PlaceBottom(ICard<S, R> card)
        {
            _cards.Insert(0, card);
        }

        public virtual void InsertRandom(ICard<S, R> card)
        {
            var i = _random.Next(0, _cards.Count + 1);
            _cards.Insert(i, card);
        }

        public virtual ICard<S, R> PeekTop()
        {
            CheckEmpty();
            return (ICard<S, R>)_cards[TopIndex].Clone();
        }

        public virtual ICard<S, R> DrawTop()
        {
            return PopAt(TopIndex);
        }

        public virtual ICard<S, R> DrawBottom()
        {
            return PopAt(BottomIndex);
        }

        public virtual ICard<S, R> Remove(S suit, R rank)
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

        protected virtual ICard<S, R> PopAt(int i)
        {
            CheckEmpty();
            var card = _cards[i];
            _cards.RemoveAt(i);
            return card;
        }
    }
}