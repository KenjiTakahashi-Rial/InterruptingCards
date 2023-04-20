using System;

namespace InterruptingCards.Models.Abstract
{
    public interface ICard<out S, out R> where S : Enum where R : Enum
    {
        S Suit { get; }
        R Rank { get; }
    }
}