using System.Collections.Generic;

using InterruptingCards.Config;
using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public interface IDeckFactory<C, D> where C : ICard where D : IDeck<C>
    {
        public static IDeckFactory<C, D> Singleton { get; }

        public D Create(IList<C> cards = null);

        public D Create(CardPack pack);
    }
}
