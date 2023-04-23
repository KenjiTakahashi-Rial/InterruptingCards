using System;

using Unity.Netcode;

namespace InterruptingCards.Models
{
    public abstract class AbstractCard<S, R> : ICard<S, R> where S : Enum where R : Enum
    {
        protected S _suit;
        protected R _rank;

        protected AbstractCard(S suit, R rank)
        {
            _suit = suit;
            _rank = rank;
        }

        public S Suit { get { return _suit; } }

        public R Rank { get { return _rank; } }

        public abstract ICard<S, R> Clone();

        public abstract void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter;
    }
}