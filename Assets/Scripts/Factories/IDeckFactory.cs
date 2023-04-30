using System.Collections.Generic;

using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public interface IDeckFactory
    {
        public static IDeckFactory Singleton { get; }

        public IDeck Prototype { get; }

        public IDeck Create(IList<ICard> cards);

        public IDeck Clone(IDeck original);
    }
}
