using System;

namespace InterruptingCards.Models.Abstract
{
    public interface IDeck<S, R> where S : Enum where R : Enum
    {
        void Shuffle();

        void PlaceTop(ICard<S, R> card);

        void PlaceBottom(ICard<S, R> card);

        void InsertRandom(ICard<S, R> card);
        ICard<S, R> DrawTop();

        ICard<S, R> DrawBottom();

        ICard<S, R> Remove(S suit, R rank);
    }
}