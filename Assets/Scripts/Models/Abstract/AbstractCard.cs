using System;

using Unity.Netcode;

namespace InterruptingCards.Models
{
    public abstract class AbstractCard<S, R> : ICard<S, R> where S : Enum where R : Enum
    {
        protected AbstractCard(S suit, R rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public virtual S Suit { get; set; }

        public virtual R Rank { get; set; }

        public abstract object Clone();

        // TODO: public abstract void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter;
    }
}