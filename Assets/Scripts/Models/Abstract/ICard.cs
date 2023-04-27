using System;

using Unity.Netcode;

namespace InterruptingCards.Models
{
    public interface ICard<S, R> : ICloneable// TODO: , INetworkSerializable
    {
        S Suit { get; set; }
        R Rank { get; set; }
    }
}