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

        IFactory Factory { get; }

        void UnsubscribeAllOnClicked();

        void UnsubscribeAllOnValueChanged();
    }
}