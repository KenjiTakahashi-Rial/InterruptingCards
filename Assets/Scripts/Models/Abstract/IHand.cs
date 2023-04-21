using System;

namespace InterruptingCards.Models
{
    public interface IHand<S, R> where S : Enum where R : Enum
    {
        int Count();

        void Add(ICard<S, R> card);

        ICard<S, R> Remove(S suit, R rank);

        ICard<S, R> Get(int i);
    }
}