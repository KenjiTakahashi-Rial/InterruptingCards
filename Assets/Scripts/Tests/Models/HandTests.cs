using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Models;
using InterruptingCards.Factories;

namespace InterruptingCards.Tests
{
    [TestFixture]
    public class HandTests
    {
        private const int DefaultCardCount = 10;
        private const int SuitStart = (int)CardSuit.Clubs;
        private const int SuitEnd = (int)CardSuit.Spades;
        private const int RankStart = (int)CardRank.Ace;
        private const int RankEnd = (int)CardRank.King;

        private readonly System.Random _random = new();

        private ICardFactory _cardFactory;
        private IHandFactory _handFactory;
        private IList<ICard> _cards;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var factoriesPrefab = Resources.Load<GameObject>("Prefabs/Factories");
            var factoriesObj = UnityEngine.Object.Instantiate(factoriesPrefab);
            _cardFactory = factoriesObj.GetComponent<ICardFactory>();
            _handFactory = factoriesObj.GetComponent<IHandFactory>();
        }

        [SetUp]
        public void SetUp()
        {
            _cards = Enumerable.Range(1, DefaultCardCount).Select(
                _ => _cardFactory.Create(
                    (CardSuit)_random.Next(SuitStart, SuitEnd), (CardRank)_random.Next(RankStart, RankEnd)
                )
            ).ToList();
        }

        [Test]
        public void TestEmpty()
        {
            var hand = new BasicHand(new List<ICard>());
            Assert.AreEqual(0, hand.Count, "Empty hand should have count 0");
        }

        [Test]
        public void TestAdd()
        {
            var hand = new BasicHand(new List<ICard>());

            for (var i = 0; i < _cards.Count; i++)
            {
                hand.Add(_cards[i]);
                Assert.AreEqual(i + 1, hand.Count, "Hand count should match number of cards added");
            }
        }

        [Test]
        public void TestRemove()
        {
            var hand = new BasicHand(_cards);

            for (var i = _cards.Count - 1; i >= 0; i--)
            {
                var card = _cards[i];
                var removed = hand.Remove(card.Suit, card.Rank);
                Assert.AreEqual(card, removed, "Card removed should match card added");
                Assert.AreEqual(hand.Count, i, "Hand count should match cards added minus cards removed");
            }
        }

        [Test]
        public void TestClone()
        {
            var hand = new BasicHand(_cards);
            var clone = _handFactory.Clone(hand);

            for (var i = 0; i < hand.Count; i++)
            {
                Assert.AreEqual(_cards[i], hand.Get(i), "Hand cards should match card added");
                Assert.AreEqual(hand.Get(i), clone.Get(i), "Clone card should match hand card");
            }
        }
    }
}