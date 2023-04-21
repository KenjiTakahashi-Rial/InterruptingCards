using System;

namespace InterruptingCards.Models
{
    public abstract class AbstractCard<S, R> : ICard<S, R> where S : Enum where R : Enum
    {
        protected AbstractCard(S suit, R rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public S Suit { get; }

        public R Rank { get; }

        public abstract ICard<S, R> Clone();
    }
}