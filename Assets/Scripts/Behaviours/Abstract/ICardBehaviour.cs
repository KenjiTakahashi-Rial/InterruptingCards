using System;

using InterruptingCards.Models;

namespace InterruptingCards.Behaviours
{
    public interface ICardBehaviour<S, R>
    {
        event Action OnCardClicked;

        bool IsFaceUp { get; set; }

        ICard<S, R> Card { get; set; }

        void UnsubscribeAllOnCardClicked();
    }
}