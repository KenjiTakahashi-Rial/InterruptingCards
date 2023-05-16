using NUnit.Framework;

using InterruptingCards.Models;

namespace InterruptingCards.Tests
{
    [TestFixture]
    public class HandTests : AbstractModelTests
    {
        private readonly HandFactory _handFactory = HandFactory.Singleton;
        
        [Test]
        public void TestEmpty()
        {
            var hand = _handFactory.Create();
            Assert.AreEqual(0, hand.Count, "Empty hand should have count 0");
        }

        [Test]
        public void TestAdd()
        {
            var hand = _handFactory.Create();

            for (var i = 0; i < _cards.Count; i++)
            {
                hand.Add(_cards[i]);
                Assert.AreEqual(i + 1, hand.Count, "Hand count should match number of cards added");
            }
        }

        [Test]
        public void TestRemove()
        {
            var hand = _handFactory.Create(_cards);

            for (var i = _cards.Count - 1; i >= 0; i--)
            {
                var card = _cards[i];
                var removed = hand.Remove(card.Id);
                Assert.AreEqual(card, removed, "Card removed should match card added");
                Assert.AreEqual(hand.Count, i, "Hand count should match cards added minus cards removed");
            }
        }
    }
}