using System;

using InterruptingCards.Config;

namespace InterruptingCards.Models
{
    public interface IHand : ICloneable
    {
        int Count { get; }

        void Add(ICard card);

        ICard Remove(int cardId);

        ICard Get(int i);

        void Clear();
    }
}