using InterruptingCards.Config;

namespace InterruptingCards.Models
{
    public interface IActiveEffect
    {
        ActiveEffect Effect { get; }
    }
}