using System;

namespace InterruptingCards.Models
{
    public abstract class AbstractPlayer<S, R> : IPlayer<S, R> where S : Enum where R : Enum
    {
        protected AbstractPlayer(ulong id, string name, IHand<S, R> hand = null)
        {
            Id = id;
            Name = name;
            Hand = hand;
        }

        public virtual ulong Id { get; }

        public virtual string Name { get; }

        public virtual IHand<S, R> Hand { get; set; }
    }
}