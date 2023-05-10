using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public interface ICardFactory
    {
        public static ICardFactory Singleton { get; }

        public ICard Create(int id);
    }
}
