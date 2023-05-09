using System;

using Unity.Netcode;

using InterruptingCards.Config;

namespace InterruptingCards.Models
{
    public interface ICard : INetworkSerializable, IEquatable<ICard>, ICloneable
    {
        string Name { get; }

        CardSuit Suit { get; }
        CardRank Rank { get; }
    }
}