using System;

using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public interface IHandManager : IHand
    {
        event Action<ICard> OnCardClicked;

        IHand Hand { get; }
    }
}
