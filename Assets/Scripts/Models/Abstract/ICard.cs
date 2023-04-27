using System;

using Unity.Netcode;

namespace InterruptingCards.Models
{
    public interface ICard : INetworkSerializable, ICloneable
    {
        SuitEnum Suit { get; set; }
        RankEnum Rank { get; set; }
    }
}