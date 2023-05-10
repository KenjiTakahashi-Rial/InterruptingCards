using InterruptingCards.Config;

namespace InterruptingCards.Models
{
    public class BasicCard : ICard
    {
        internal BasicCard(int id, CardSuit suit, CardRank rank, string name)
        {
            Id = id;
            Suit = suit;
            Rank = rank;
            Name = name;
        }

        public virtual int Id { get; }

        public virtual CardSuit Suit { get; }

        public virtual CardRank Rank { get; }

        public virtual string Name { get; }

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