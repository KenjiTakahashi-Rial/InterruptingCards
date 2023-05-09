using System.Collections.Generic;

using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public interface IHandFactory
    {
        public static IHandFactory Singleton { get; }

        public IHand Create(IList<ICard> cards = null);

        public IHand Clone(IHand original);
    }
}
