using System;

using Unity.Netcode;

namespace InterruptingCards.Models
{
    public interface IHand : ICloneable
    {
        int Count { get; }

        void Add(ICard card);

        ICard Remove(SuitEnum suit, RankEnum rank);

        ICard Get(int i);
    }
}