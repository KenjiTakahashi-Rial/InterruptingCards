using System;

using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public interface IDeckManager<S, R> : IDeck<S, R> where S: Enum where R : Enum
    {
        event Action OnDeckClicked;

        void ResetDeck();
    }
}
