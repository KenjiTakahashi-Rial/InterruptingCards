using InterruptingCards.Config;

namespace InterruptingCards.Models
{
    [System.Serializable]
    public class BasicCard : ICard
    {
        public virtual int Id => CardConfig.CardId(this);
        
        public virtual string Name { get; }

        public virtual CardSuit Suit { get; }

        public virtual CardRank Rank { get; }

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