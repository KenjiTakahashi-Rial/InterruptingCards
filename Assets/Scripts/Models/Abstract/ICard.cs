using System;

using Unity.Netcode;

namespace InterruptingCards.Models
{
    public interface ICard<out S, out R> : ICloneable, INetworkSerializable
    {
        S Suit { get; }
        R Rank { get; }
    }
}