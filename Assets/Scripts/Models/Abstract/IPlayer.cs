using System;

namespace InterruptingCards.Models.Abstract
{
    public interface IPlayer<S, R> where S : Enum where R : Enum
    {
        ulong Id { get; }

        string Name { get; }

        IHand<S, R> Hand { get; set; }
    }
}