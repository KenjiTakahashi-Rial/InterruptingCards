using System;

using InterruptingCards.Models;

namespace InterruptingCards.Behaviours
{
    public interface IActiveCardBehaviour<C> : ICardBehaviour<C> where C : ICard
    {
        event Action OnActivated;
        new IActiveCard Card { get; set; }
    }
}