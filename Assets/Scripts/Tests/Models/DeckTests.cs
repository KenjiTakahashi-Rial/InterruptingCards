using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using InterruptingCards.Models;
using InterruptingCards.Factories;

namespace InterruptingCards.Tests
{
    [TestFixture]
    public class DeckTests
    {
        private const int DefaultCardCount = 10;
        private const int SuitStart = (int)SuitEnum.Clubs;
        private const int SuitEnd = (int)SuitEnum.Spades;
        private const int RankStart = (int)RankEnum.Ace;
        private const int RankEnd = (int)RankEnum.King;

        private readonly System.Random _random = new();

        private ICardFactory _cardFactory;
        private IDeckFactory _deckFactory;
        private IList<ICard> _cards;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var factoriesObj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Factories"));
            _cardFactory = factoriesObj.GetComponent<ICardFactory>();
            _deckFactory = factoriesObj.GetComponent<IDeckFactory>();
        }

        [SetUp]
        public void SetUp()
        {
            _cards = Enumerable.Range(1, DefaultCardCount).Select(
                _ => _cardFactory.Create(
                    (SuitEnum)_random.Next(SuitStart, SuitEnd), (RankEnum)_random.Next(RankStart, RankEnd)
                )
            ).ToList();
        }

        [UnityTest]
        public IEnumerator TestShuffle()
        {
            while (_deckFactory.Prototype == null)
            {
                yield return null;
            }

            var deck = _deckFactory.Prototype;
            deck.Shuffle();
            Assert.AreEqual(_deckFactory.Prototype.Count, deck.Count, "Deck should match prototype count after shuffling");
        }

        [Test]
        public void TestTop()
        {
            var deck = _deckFactory.Create(new List<ICard>());

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
            var deck = _deckFactory.Create(new List<ICard>());

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

        [UnityTest]
        public IEnumerator TestInsertRemove()
        {
            while (_deckFactory.Prototype == null)
            {
                yield return null;
            }

            var deck = _deckFactory.Prototype;
            var count = deck.Count;

            for (var i = 0; i < _cards.Count; i++)
            {
                var card = _cards[i];
                deck.InsertRandom(_cards[i]);
                Assert.AreEqual(count + 1, deck.Count, "Deck count should match original count plus one after insertion");
                Assert.AreEqual(card, deck.Remove(card.Suit, card.Rank), "Top peeked card should match first placed card");
                Assert.AreEqual(count, deck.Count, "Deck count should match original count after removal");
            }
        }
    }
}