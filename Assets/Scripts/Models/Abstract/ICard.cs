using System;

using Unity.Netcode;

using InterruptingCards.Config;

namespace InterruptingCards.Models
{
    public interface ICard : INetworkSerializable, IEquatable<ICard>, ICloneable
    {
        CardSuit Suit { get; }
        CardRank Rank { get; }
    }
}