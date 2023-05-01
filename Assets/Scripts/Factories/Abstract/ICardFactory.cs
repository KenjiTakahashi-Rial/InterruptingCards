using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public interface ICardFactory
    {
        public static ICardFactory Singleton { get; }

        public ICard Create(SuitEnum suit, RankEnum rank);

        public ICard Clone(ICard original);
    }
}
