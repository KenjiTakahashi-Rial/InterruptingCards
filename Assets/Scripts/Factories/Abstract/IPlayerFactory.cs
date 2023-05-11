using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public interface IPlayerFactory<C, H, P> where C : ICard where H : IHand<C> where P : IPlayer<C, H>
    {
        public static IPlayerFactory<C, H, P> Singleton { get; }

        public P Create(ulong id, string name);
    }
}
