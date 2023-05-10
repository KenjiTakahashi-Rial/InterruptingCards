using System;

using InterruptingCards.Models;

namespace InterruptingCards.Behaviours
{
    public interface IActiveCardBehaviour : ICardBehaviour
    {
        event Action OnActivated;
        new ICard Card { get; set; }
    }
}