using System;

using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public interface IDeckManager : IDeck
    {
        event Action OnDeckClicked;

        void ResetDeck();
    }
}
