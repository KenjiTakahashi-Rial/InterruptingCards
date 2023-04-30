using System;

using InterruptingCards.Models;

namespace InterruptingCards.Behaviours
{ 
    public interface ICardBehaviour
    {
        event Action OnCardClicked;

        bool IsFaceUp { get; set; }

        ICard Card { get; set; }

        void UnsubscribeAllOnCardClicked();
    }
}