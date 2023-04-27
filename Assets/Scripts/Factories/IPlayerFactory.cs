using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public interface IPlayerFactory
    {
        public static IPlayerFactory Singleton { get; }

        public IPlayer Create(ulong id, string name, IHand hand = null);
    }
}
