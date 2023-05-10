using InterruptingCards.Config;

namespace InterruptingCards.Models
{
    [System.Serializable]
    public class MetadataCard
    {
        public CardSuit Suit { get; }

        public CardRank Rank { get; }

        public int Count { get; }
    }
}