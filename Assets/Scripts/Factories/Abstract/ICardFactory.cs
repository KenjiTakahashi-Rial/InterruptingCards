using InterruptingCards.Config;
using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public interface ICardFactory
    {
        public static ICardFactory Singleton { get; }

        public ICard Create(CardSuit suit, CardRank rank);

        public ICard Clone(ICard original);
    }
}
