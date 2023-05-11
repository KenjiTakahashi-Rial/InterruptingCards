using System;

using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public interface IHandManager<C, H> : IHand<C> where C : ICard where H : IHand<C>
    {
        event Action<C> OnCardClicked;

        H Hand { get; }
    }
}
