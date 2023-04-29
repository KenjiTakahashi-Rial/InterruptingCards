using System;

using Unity.Netcode;

namespace InterruptingCards.Models
{
    public abstract class AbstractCard : ICard
    {
        protected SuitEnum _suit;
        protected RankEnum _rank;

        protected AbstractCard() { }

        protected AbstractCard(SuitEnum suit, RankEnum rank)
        {
            _suit = suit;
            _rank = rank;
        }

// The inner fields are necessary for network serialization
#pragma warning disable S2292 // Trivial properties should be auto-implemented
        public virtual SuitEnum Suit { get => _suit; set => _suit = value; }

        public virtual RankEnum Rank { get => _rank; set => _rank = value; }
#pragma warning restore S2292 // Trivial properties should be auto-implemented

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _suit);
            serializer.SerializeValue(ref _rank);
        }

        public abstract object Clone();

        public override string ToString()
        {
            return $"{Enum.GetName(typeof(RankEnum), _rank)} | {Enum.GetName(typeof(SuitEnum), _suit)}";
        }
    }
}