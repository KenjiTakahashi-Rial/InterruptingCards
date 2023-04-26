using System;

using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public interface ICardFactory<S, R> where S : Enum where R : Enum
    {
        public static ICardFactory<S, R> Singleton { get; }

        public ICard<S, R> Create(S suit, R rank);

        public ICard<S, R> Clone(ICard<S, R> original);
    }
}
