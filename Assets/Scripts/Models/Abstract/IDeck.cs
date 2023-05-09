using System;

using InterruptingCards.Config;

namespace InterruptingCards.Models
{
    public interface IDeck : ICloneable
    {
        int Count { get; }

        void Shuffle();

        void PlaceTop(ICard card);

        void PlaceBottom(ICard card);

        void InsertRandom(ICard card);

        ICard PeekTop();

        ICard DrawTop();

        ICard DrawBottom();

        ICard Remove(CardSuit suit, CardRank rank);
    }
}