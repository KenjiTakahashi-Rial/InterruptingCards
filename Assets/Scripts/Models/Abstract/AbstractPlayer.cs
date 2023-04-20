using System;

namespace InterruptingCards.Models.Abstract
{
    public abstract class AbstractPlayer<S, R> : IPlayer<S, R> where S : Enum where R : Enum
    {
        protected AbstractPlayer(ulong id, string name, IHand<S, R> hand = null)
        {
            Id = id;
            Name = name;
            Hand = hand;
        }

        public ulong Id { get; }

        public string Name { get; }

        public IHand<S, R> Hand { get; set; }
    }
}