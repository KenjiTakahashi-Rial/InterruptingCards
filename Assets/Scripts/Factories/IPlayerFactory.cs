using System;

using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public interface IPlayerFactory<S, R> where S : Enum where R : Enum
    {
        public static IPlayerFactory<S, R> Singleton { get; }

        public IPlayer<S, R> Create(ulong id, string name, IHand<S, R> hand = null);
    }
}
