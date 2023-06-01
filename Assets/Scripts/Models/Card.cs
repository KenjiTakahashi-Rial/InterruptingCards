using InterruptingCards.Config;

namespace InterruptingCards.Models
{
    public class Card
    {
        public Card(ParserCard metadataCard)
        {
            Id = metadataCard.Id;
            Suit = metadataCard.Suit;
            Rank = metadataCard.Rank;
            Name = metadataCard.Name;
            Count = metadataCard.Count;
            ActiveEffect = metadataCard.ActiveEffect;
        }

        // All attributes must be readonly

        // TODO: Make an instance ID too
        public int Id { get; }

        public CardSuit Suit { get; }

        public CardRank Rank { get; }

        public string Name { get; }

        public int Count { get; }

        public CardActiveEffect ActiveEffect { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}