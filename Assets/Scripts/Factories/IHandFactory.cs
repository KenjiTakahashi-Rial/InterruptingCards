using System;
using System.Collections.Generic;

using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public interface IHandFactory<S, R> where S : Enum where R : Enum
    {
        public static IHandFactory<S, R> Singleton { get; }

        public IHand<S, R> Create(IList<ICard<S, R>> cards);

        public IHand<S, R> Clone(IHand<S, R> original);
    }
}
