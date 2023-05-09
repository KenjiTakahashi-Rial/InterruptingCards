using System;

using Unity.Netcode;

using InterruptingCards.Config;

namespace InterruptingCards.Models
{
    public class BasicCard : ICard
    {
        protected CardSuit _suit;
        protected CardRank _rank;

        // The empty constructor is necessary for network serialization
        public BasicCard() { }

        public BasicCard(CardSuit suit, CardRank rank)
        {
            _suit = suit;
            _rank = rank;
        }

        public virtual CardSuit Suit => _suit;

        public virtual CardRank Rank => _rank;

        public virtual bool Equals(ICard other)
        {
            return other != null && _suit == other.Suit && _rank == other.Rank;
        }

        public virtual void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
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
            return $"{Enum.GetName(typeof(CardRank), _rank)} | {Enum.GetName(typeof(CardSuit), _suit)}";
        }
    }
}