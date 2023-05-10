using InterruptingCards.Config;

namespace InterruptingCards.Models
{
    [System.Serializable]
    public class BasicCard : ICard
    {
        // This should never be called directly. Use a card factory instead
        public BasicCard(string name, CardSuit suit, CardRank rank)
        {
            Name = name;
            Suit = suit;
            Rank = rank;
        }

        public virtual string Name { get; }

        public virtual CardSuit Suit { get; }

        public virtual CardRank Rank { get; }

        public virtual bool Equals(ICard other)
        {
            return CardConfig.CardId(this) == CardConfig.CardId(other);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}