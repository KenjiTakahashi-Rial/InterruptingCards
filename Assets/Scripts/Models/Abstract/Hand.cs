using System;
using System.Collections.Generic;

using InterruptingCards.Models.PlayingCards;

namespace InterruptingCards.Models.Abstract
{
    public abstract class Hand<S, R> : IHand<S, R> where S : Enum where R : Enum
    {
        protected readonly IList<ICard<S, R>> _cards;

        public Hand(IList<ICard<S, R>> cards = null)
        {
            if (cards == null)
            {
                return;
            }

            _cards = cards;
        }

        public void Insert(int i, ICard<S, R> card)
        {
            _cards.Insert(i, card);
        }

        public void Add(ICard<S, R> card)
        {
            Insert(0, card);
        }

        public ICard<S, R> Remove(S suit, R rank)
        {
            return Utilities.Remove(_cards, suit, rank);
        }
    }
}