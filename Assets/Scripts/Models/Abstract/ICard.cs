using System;

using InterruptingCards.Config;

namespace InterruptingCards.Models
{
    public interface ICard : IEquatable<ICard>
    {
        int Id { get; }

        CardSuit Suit { get; }

        CardRank Rank { get; }

        string Name { get; }
    }
}