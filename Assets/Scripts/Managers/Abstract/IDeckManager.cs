using System;

using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public interface IDeckManager : IDeck
    {
        event Action OnDeckClicked;

        bool IsFaceUp { get; set; }

        void Initialize();

        void Clear();
    }
}
