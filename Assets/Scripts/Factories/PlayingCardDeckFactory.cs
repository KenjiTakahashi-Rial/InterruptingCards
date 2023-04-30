using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public class PlayingCardDeckFactory : MonoBehaviour, IDeckFactory
    {
        private static readonly ICollection<SuitEnum> Suits =
            Enum.GetValues(typeof(SuitEnum))
                .Cast<SuitEnum>()
                .Where(e => e >= SuitEnum.Clubs && e <= SuitEnum.Spades)
                .ToList();

        private static readonly ICollection<RankEnum> Ranks =
            Enum.GetValues(typeof(RankEnum))
                .Cast<RankEnum>()
                .Where(e => e >= RankEnum.Ace && e <= RankEnum.King)
                .ToList();

        private IDeck _prototype;

        public static IDeckFactory Singleton { get; private set; }

        public IDeck Prototype { get => Clone(_prototype); set => _prototype = value; }

        private ICardFactory CardFactory => PlayingCardFactory.Singleton;

        public IDeck Create(IList<ICard> cards)
        {
            return new PlayingCardDeck(cards);
        }

        public IDeck Clone(IDeck original)
        {
            return (PlayingCardDeck)original.Clone();
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
