namespace InterruptingCards.Models
{
    public abstract class AbstractPlayer : IPlayer
    {
        protected AbstractPlayer(ulong id, string name, IHand hand = null)
        {
            Id = id;
            Name = name;
            Hand = hand;
        }

        public virtual ulong Id { get; }

        public virtual string Name { get; }

        public virtual IHand Hand { get; set; }
    }
}