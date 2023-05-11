using System;

using InterruptingCards.Config;
using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public interface IDeckManager<C> : IDeck<C> where C : ICard
    {
        event Action OnDeckClicked;

        bool IsFaceUp { get; set; }

        void Initialize(CardPack cardPack);

        void Clear();
    }
}
