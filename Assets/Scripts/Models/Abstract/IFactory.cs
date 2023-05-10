using System.Collections.Generic;

namespace InterruptingCards.Models
{
    public interface IFactory
    {
        public static IFactory Singleton { get; }

        public IPlayer CreatePlayer(ulong id, string name, IHand hand = null);

        public ICard CreateCard(int id);

        public IDeck CreateDeck(IList<ICard> cards = null);

        public IDeck CreateFullDeck();

        public IHand CreateHand(IList<ICard> cards = null);
    }
}
