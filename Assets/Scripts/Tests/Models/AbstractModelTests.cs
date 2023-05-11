using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using InterruptingCards.Config;
using InterruptingCards.Factories;
using InterruptingCards.Models;

namespace InterruptingCards.Tests
{
    [TestFixture]
    public abstract class AbstractModelTests
    {
        protected const int DefaultCardCount = 10;
        protected const CardPack Pack = CardPack.PlayingCards;

        protected readonly ICardFactory<BasicCard> _cardFactory = BasicCardFactory.Singleton;
        protected readonly System.Random _random = new();

        protected IList<BasicCard> _cards;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _cardFactory.Load(Pack);
        }

        [SetUp]
        public void SetUp()
        {
            _cards = CardConfig.Singleton.GetCardPack(Pack)
                .OrderBy(c => _random.Next())
                .Take(DefaultCardCount)
                .Select(c => _cardFactory.Create(c.Id))
                .ToList();
        }
    }
}