using System;

namespace InterruptingCards.Models
{
    public interface IDeck<S, R> : ICloneable where S : Enum where R : Enum
    {
        int Count();

        void Shuffle();

        void PlaceTop(ICard<S, R> card);

        void PlaceBottom(ICard<S, R> card);

        void InsertRandom(ICard<S, R> card);

        ICard<S, R> PeekTop();

        ICard<S, R> DrawTop();

        ICard<S, R> DrawBottom();

        ICard<S, R> Remove(S suit, R rank);
    }
}