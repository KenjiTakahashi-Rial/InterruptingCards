using InterruptingCards.Config;
using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public interface ICardFactory<C> where C : ICard
    {
        static ICardFactory<C> Singleton { get; }

        void Load(CardPack cardPack);

        C Create(int id);
    }
}
