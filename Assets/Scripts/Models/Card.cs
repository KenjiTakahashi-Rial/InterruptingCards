namespace InterruptingCards.Models
{
    public class Card
    {
        public Card(int id, MetadataCard metadataCard)
        {
            Id = id;
            Suit = metadataCard.Suit;
            Rank = metadataCard.Rank;
            Name = metadataCard.Name;
            Value = metadataCard.Value;
            LootAbility = metadataCard.LootAbility;
            ActivatedAbility = metadataCard.ActivatedAbility;
        }

        // All attributes must be readonly

        // Instance ID. Each card will have a unique ID, even if they have the same suit & rank
        public int Id { get; }

        public CardSuit Suit { get; }

        public CardRank Rank { get; }

        public string Name { get; }

        public int Value { get; }

        public CardAbility LootAbility { get; }

        public CardAbility ActivatedAbility { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}