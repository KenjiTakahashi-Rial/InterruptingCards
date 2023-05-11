using System.Collections.Generic;

using InterruptingCards.Config;
using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public class BasicHandFactory : IHandFactory<BasicCard, BasicHand>
    {
        protected static readonly IHandFactory<BasicCard, BasicHand> Instance = new BasicHandFactory();
        protected readonly Dictionary<CardPack, IList<BasicCard>> _packs = new();

        private BasicHandFactory() { }

        public static IHandFactory<BasicCard, BasicHand> Singleton { get { return Instance; } }

        public BasicHand Create(IList<BasicCard> cards = null)
        {
            return cards == null ? new BasicHand(new List<BasicCard>()) : new BasicHand(cards);
        }
    }
}
