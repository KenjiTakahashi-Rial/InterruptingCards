using System;

namespace InterruptingCards.Models
{
    public interface IDeck : ICloneable
    {
        int Count();

        void Shuffle();

        void PlaceTop(ICard card);

        void PlaceBottom(ICard card);

        void InsertRandom(ICard card);

        ICard PeekTop();

        ICard DrawTop();

        ICard DrawBottom();

        ICard Remove(SuitEnum suit, RankEnum rank);
    }
}