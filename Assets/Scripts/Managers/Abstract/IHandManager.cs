using System;

using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public interface IHandManager<S, R> : IHand<S, R> where S: Enum where R : Enum
    {
        event Action<ICard<S, R>> OnCardClicked;
    }
}
