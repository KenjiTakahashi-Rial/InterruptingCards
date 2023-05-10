using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NUnit.Framework;
using UnityEngine;

using InterruptingCards.Models;
using UnityEngine.TestTools;

namespace InterruptingCards.Tests
{
    [TestFixture]
    public class HandTests
    {
        private const int DefaultCardCount = 10;

        private readonly BindingFlags _bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        private readonly System.Random _random = new();

        private IFactory _factory;
        private IList<ICard> _cards;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var basicGameManagerPrefab = Resources.Load<GameObject>("Prefabs/BasicGameManager");
            var basicGameManagerObj = Object.Instantiate(basicGameManagerPrefab);
            _factory = basicGameManagerObj.GetComponent<IFactory>();
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
                var removed = hand.Remove(card.Id);
                Assert.AreEqual(card, removed, "Card removed should match card added");
                Assert.AreEqual(hand.Count, i, "Hand count should match cards added minus cards removed");
            }
        }
    }
}