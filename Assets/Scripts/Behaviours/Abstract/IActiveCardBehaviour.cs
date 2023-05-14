using System;

using InterruptingCards.Models;

namespace InterruptingCards.Behaviours
{
    public interface IActiveCardBehaviour<C> : ICardBehaviour<C> where C : ICard
    {
        event Action OnActivated;
        new C Card { get; set; }

        void UnsubscribeAllOnActivated();
    }
}