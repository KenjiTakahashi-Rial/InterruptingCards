using System;

using Unity.Netcode;

using InterruptingCards.Config;

namespace InterruptingCards.Models
{
    public interface ICard : IEquatable<ICard>
    {
        string Name { get; }

        CardSuit Suit { get; }
        CardRank Rank { get; }
    }
}