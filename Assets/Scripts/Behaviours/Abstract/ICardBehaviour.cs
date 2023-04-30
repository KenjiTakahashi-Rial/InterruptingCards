using System;

using InterruptingCards.Models;

namespace InterruptingCards.Behaviours
{ 
    public interface ICardBehaviour
    {
        event Action OnClicked;

        event Action OnValueChanged;

        bool IsFaceUp { get; set; }

        ICard Card { get; set; }

        void UnsubscribeAllOnClicked();

        void UnsubscribeAllOnValueChanged();
    }
}