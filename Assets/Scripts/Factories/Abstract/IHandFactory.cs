using System.Collections.Generic;

using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public interface IHandFactory<C, H> where C : ICard where H : IHand<C>
    {
        public static IHandFactory<C, H> Singleton { get; }

        public H Create(IList<C> cards = null);
    }
}
