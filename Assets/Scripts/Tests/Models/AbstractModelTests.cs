using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using InterruptingCards.Config;
using InterruptingCards.Models;

namespace InterruptingCards.Tests
{
    [TestFixture]
    public abstract class AbstractModelTests
    {
        protected const int DefaultCardCount = 10;
        protected const CardPack Pack = CardPack.PlayingCards;

        protected readonly CardFactory _cardFactory = CardFactory.Singleton;
        protected readonly System.Random _random = new();

        protected IList<Card> _cards;

        [OneTimeSetUp]
        public virtual void OneTimeSetUp()
        {
            _cardFactory.Load(Pack);
        }

        [SetUp]
        public virtual void SetUp()
        {
            _cards = CardConfig.Singleton.GetCardPack(Pack)
                .OrderBy(c => _random.Next())
                .Take(DefaultCardCount)
                .Select(c => _cardFactory.Create(c.Id))
                .ToList();
        }
    }
}