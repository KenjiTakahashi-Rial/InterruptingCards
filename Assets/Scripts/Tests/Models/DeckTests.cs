using NUnit.Framework;

using InterruptingCards.Models;
using InterruptingCards.Factories;

namespace InterruptingCards.Tests
{
    [TestFixture]
    public class DeckTests : AbstractModelTests
    {
        private readonly IDeckFactory<BasicCard, BasicDeck> _deckFactory = BasicDeckFactory.Singleton;

        [Test]
        public void TestShuffle()
        {
            var deck = _deckFactory.Create(Pack);
            var count = deck.Count;
            deck.Shuffle();
            Assert.AreEqual(count, deck.Count, "Deck should match prototype count after shuffling");
        }

        [Test]
        public void TestTop()
        {
            var deck = _deckFactory.Create();

            for (var i = 0; i < _cards.Count; i++)
            {
                deck.PlaceTop(_cards[i]);
                Assert.AreEqual(_cards[i], deck.PeekTop(), "Top peeked card should match placed card");
                Assert.AreEqual(i + 1, deck.Count, "Deck count should match placed cards");
            }

            for (var i = _cards.Count - 1; i >= 0; i--)
            {
                Assert.AreEqual(_cards[i], deck.DrawTop(), "Top drawn card should match placed card");
                Assert.AreEqual(i, i, "Deck count should match placed cards minus drawn cards");
            }
        }

        [Test]
        public void TestBottom()
        {
            var deck = _deckFactory.Create();

            for (var i = 0; i < _cards.Count; i++)
            {
                deck.PlaceBottom(_cards[i]);
                Assert.AreEqual(_cards[0], deck.PeekTop(), "Top peeked card should match first placed card");
                Assert.AreEqual(i + 1, deck.Count, "Deck count should match placed cards");
            }

            for (var i = _cards.Count - 1; i >= 0; i--)
            {
                Assert.AreEqual(_cards[i], deck.DrawBottom(), "Bottom drawn card should match placed card");
                Assert.AreEqual(i, i, "Deck count should match placed cards minus drawn cards");
            }
        }

        [Test]
        public void TestInsertRemove()
        {
            var deck = _deckFactory.Create(Pack);
            var count = deck.Count;

            for (var i = 0; i < _cards.Count; i++)
            {
                var card = _cards[i];
                deck.InsertRandom(_cards[i]);
                Assert.AreEqual(count + 1, deck.Count, "Deck count should match original count plus one after insertion");
                Assert.AreEqual(card, deck.Remove(card.Id), "Top peeked card should match first placed card");
                Assert.AreEqual(count, deck.Count, "Deck count should match original count after removal");
            }
        }
    }
}