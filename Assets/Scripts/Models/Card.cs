using InterruptingCards.Config;

namespace InterruptingCards.Models
{
    public class Card
    {
        // Do not call directly; use a factory
        internal Card(MetadataCard metadataCard)
        {
            Id = metadataCard.Id;
            Suit = metadataCard.Suit;
            Rank = metadataCard.Rank;
            Name = metadataCard.Name;
            ActiveEffect = metadataCard.ActiveEffect;
        }

        public int Id { get; }

        public CardSuit Suit { get; }

        public CardRank Rank { get; }

        public string Name { get; }

        public CardActiveEffect ActiveEffect { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}