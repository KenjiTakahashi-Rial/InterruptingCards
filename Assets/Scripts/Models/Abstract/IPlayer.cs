using System;

namespace InterruptingCards.Models
{
    public interface IPlayer<S, R> where S : Enum where R : Enum
    {
        ulong Id { get; }

        string Name { get; }

        IHand<S, R> Hand { get; set; }
    }
}