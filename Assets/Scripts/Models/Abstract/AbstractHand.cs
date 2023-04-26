using System;
using System.Collections.Generic;

namespace InterruptingCards.Models
{
    public abstract class AbstractHand<S, R> : IHand<S, R> where S : Enum where R : Enum
    {
        protected readonly IList<ICard<S, R>> _cards;

        protected AbstractHand(IList<ICard<S, R>> cards)
        {
            _cards = cards;
        }

        public virtual int Count()
        {
            return _cards.Count;
        }

        public virtual void Insert(int i, ICard<S, R> card)
        {
            _cards.Insert(i, card);
        }

        public virtual void Add(ICard<S, R> card)
        {
            Insert(0, card);
        }

        public virtual ICard<S, R> Remove(S suit, R rank)
        {
            return Utilities.Remove(_cards, suit, rank);
        }

        public virtual ICard<S, R> Get(int i)
        {
            return _cards[i];
        }

        public abstract object Clone();
    }
}