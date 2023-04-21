using System;

namespace InterruptingCards.Models
{
    public interface ICard<out S, out R> where S : Enum where R : Enum
    {
        S Suit { get; }
        R Rank { get; }

        ICard<S, R> Clone();
    }
}