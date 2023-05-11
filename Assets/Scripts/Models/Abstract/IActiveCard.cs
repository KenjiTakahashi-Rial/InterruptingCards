
using InterruptingCards.Config;

namespace InterruptingCards.Models
{
    public interface IActiveCard : ICard
    {
        CardActiveEffect ActiveEffect { get; }
    }
}