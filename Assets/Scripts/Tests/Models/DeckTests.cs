using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using InterruptingCards.Models;

namespace InterruptingCards.Tests
{
    [TestFixture]
    public class DeckTests
    {
        private const int DefaultCardCount = 10;

        private readonly BindingFlags _bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        private readonly System.Random _random = new();

        private IFactory _factory;
        private IList<ICard> _cards;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var basicFactoryPrefab = Resources.Load<GameObject>("Prefabs/BasicFactory");
            var basicFactoryObj = Object.Instantiate(basicFactoryPrefab);
            _factory = basicFactoryObj.GetComponent<IFactory>();
        }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            var allCardsFieldInfo = typeof(BasicFactory).GetField("_cards", _bindingFlags);
            var allCards = (ImmutableDictionary<int, BasicCard>)allCardsFieldInfo.GetValue(_factory);

            while (allCards == null)
            {
                allCards = (ImmutableDictionary<int, BasicCard>)allCardsFieldInfo.GetValue(_factory);
                yield return null;
            }
            
            _cards = allCards.Values.OrderBy(x => _random.Next()).Take(DefaultCardCount).Cast<ICard>().ToList();
        }

        [Test]
        public void TestShuffle()
        {
            var deck = _factory.CreateFullDeck();
            var count = deck.Count;
            deck.Shuffle();
            Assert.AreEqual(count, deck.Count, "Deck should match prototype count after shuffling");
        }

        [Test]
        public void TestTop()
        {
            var deck = _factory.CreateDeck();

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
            var deck = _factory.CreateDeck();

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
            var deck = _factory.CreateFullDeck();
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