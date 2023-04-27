using Unity.Netcode;

namespace InterruptingCards.Models
{
    public abstract class AbstractCard : ICard
    {
        protected SuitEnum _suit;
        protected RankEnum _rank;

        protected AbstractCard(SuitEnum suit, RankEnum rank)
        {
            _suit = suit;
            _rank = rank;
        }

        public virtual SuitEnum Suit { get => _suit; set => _suit = value; }

        public virtual RankEnum Rank { get => _rank; set => _rank = value; }

        public abstract object Clone();

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _suit);
            serializer.SerializeValue(ref _rank);
        }
    }
}