using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public class BasicDeckFactory : MonoBehaviour, IDeckFactory
    {
        private static readonly ICollection<CardSuit> Suits =
            Enum.GetValues(typeof(CardSuit))
                .Cast<CardSuit>()
                .Where(e => e >= CardSuit.Clubs && e <= CardSuit.Spades)
                .ToList();

        private static readonly ICollection<CardRank> Ranks =
            Enum.GetValues(typeof(CardRank))
                .Cast<CardRank>()
                .Where(e => e >= CardRank.Ace && e <= CardRank.King)
                .ToList();

        protected IDeck _prototype;

        public static IDeckFactory Singleton { get; private set; }

        public IDeck Prototype { get => _prototype == null ? null : Clone(_prototype); set => _prototype = value; }

        private ICardFactory CardFactory => BasicCardFactory.Singleton;

        public IDeck Create(IList<ICard> cards)
        {
            return new BasicDeck(cards);
        }

        public IDeck Clone(IDeck original)
        {
            return (BasicDeck)original.Clone();
        }

        private void Awake()
        {
            Singleton = this;
        }

        private void Start()
        {
            // Must happen after Awake so card factory singleton is ready
            var cards = new List<ICard>(Suits.Count * Ranks.Count);

            foreach (var suit in Suits)
            {
                foreach (var rank in Ranks)
                {
                    cards.Add(CardFactory.Create(suit, rank));
                }
            }

            _prototype = Create(cards);
        }

        private void OnDestroy()
        {
            Singleton = null;
        }
    }
}
