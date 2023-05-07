using System;

using Unity.Netcode;

namespace InterruptingCards.Models
{
    public class BasicCard : ICard
    {
        protected SuitEnum _suit;
        protected RankEnum _rank;

        // The empty constructor is necessary for network serialization
        public BasicCard() { }

        public BasicCard(SuitEnum suit, RankEnum rank)
        {
            _suit = suit;
            _rank = rank;
        }

// The inner fields are necessary for network serialization
#pragma warning disable S2292 // Trivial properties should be auto-implemented
        public virtual SuitEnum Suit => _suit;

        public virtual RankEnum Rank => _rank;
#pragma warning restore S2292 // Trivial properties should be auto-implemented



        public virtual bool Equals(ICard other)
        {
            return other != null && _suit == other.Suit && _rank == other.Rank;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _suit);
            serializer.SerializeValue(ref _rank);
        }

        public object Clone()
        {
            return new BasicCard(_suit, _rank);
        }

        public override string ToString()
        {
            return $"{Enum.GetName(typeof(RankEnum), _rank)} | {Enum.GetName(typeof(SuitEnum), _suit)}";
        }
    }
}