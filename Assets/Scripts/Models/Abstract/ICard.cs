using System;

using Unity.Netcode;

namespace InterruptingCards.Models
{
    public interface ICard : INetworkSerializable, IEquatable<ICard>, ICloneable
    {
        SuitEnum Suit { get; }
        RankEnum Rank { get; }
    }
}