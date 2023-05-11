using System;

using InterruptingCards.Models;

namespace InterruptingCards.Behaviours
{ 
    public interface ICardBehaviour<C> where C : ICard
    {
        event Action OnClicked;

        event Action OnValueChanged;

        bool IsFaceUp { get; set; }

        C Card { get; set; }

        void UnsubscribeAllOnClicked();

        void UnsubscribeAllOnValueChanged();
    }
}