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

        public virtual S Suit { get => _suit; }

        public virtual R Rank { get => _rank; }

        public abstract object Clone();

        public abstract void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter;
    }
}