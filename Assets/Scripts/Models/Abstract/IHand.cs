using System;

namespace InterruptingCards.Models.Abstract
{
    public interface IHand<S, R> where S : Enum where R : Enum
    {
        void Add(ICard<S, R> card);

        ICard<S, R> Remove(S suit, R rank);
    }
}