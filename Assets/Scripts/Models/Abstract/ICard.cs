using System;

using Unity.Netcode;

namespace InterruptingCards.Models
{
    public interface ICard : ICloneable, INetworkSerializable
    {
        SuitEnum Suit { get; set; }
        RankEnum Rank { get; set; }
    }
}