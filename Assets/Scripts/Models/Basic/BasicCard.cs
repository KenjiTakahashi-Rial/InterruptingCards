using InterruptingCards.Config;

namespace InterruptingCards.Models
{
    public class BasicCard : ICard
    {
        public BasicCard(MetadataCard metadataCard)
        {
            Id = metadataCard.Id;
            Suit = metadataCard.Suit;
            Rank = metadataCard.Rank;
            Name = metadataCard.Name;
        }

        public virtual int Id { get; private set; }

        public virtual CardSuit Suit { get; private set; }

        public virtual CardRank Rank { get; private set; }

        public virtual string Name { get; private set; }

        public virtual bool Equals(ICard other)
        {
            return Id == other.Id;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}