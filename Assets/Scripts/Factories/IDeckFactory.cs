using System;
using System.Collections.Generic;

using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public interface IDeckFactory<S, R> where S : Enum where R : Enum
    {
        public static IDeckFactory<S, R> Singleton { get; }

        public IDeck<S, R> Create(IList<ICard<S, R>> cards);

        public IDeck<S, R> Clone(IDeck<S, R> original);
    }
}
