using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using InterruptingCards.Config;

namespace InterruptingCards.Models
{
    public class DeckFactory
    {
        private readonly CardConfig _cardConfig = CardConfig.Singleton;
        private readonly CardFactory _cardFactory = CardFactory.Singleton;
        private readonly Dictionary<CardPack, IList<Card>> _packs = new();

        private DeckFactory() { }

        public static DeckFactory Singleton { get; } = new DeckFactory();

        public Deck Create(IList<Card> cards = null)
        {
            return cards == null ? new Deck(new List<Card>()) : new Deck(cards);
        }

        public Deck Create(CardPack cardPack)
        {
            if (!_packs.ContainsKey(cardPack))
            {
                Debug.LogWarning($"Factory does not contain {cardPack}. Is the card pack loaded?");
                return null;
            }

            return Create(_packs[cardPack]);
        }

        public void Load(CardPack cardPack)
        {
            _cardFactory.Load(cardPack);

            if (_packs.ContainsKey(cardPack))
            {
                Debug.Log($"Factory already loaded {cardPack}");
                return;
            }

            Debug.Log($"Factory loading {cardPack}");

            var cards = new List<Card>();
            var pack = _cardConfig.GetCardPack(cardPack);

            foreach (var id in pack.Select(c => c.Id))
            {
                for (var i = 0; i < _cardConfig.GetMetadataCard(id).Count; i++)
                {
                    cards.Add(_cardFactory.Create(id));
                }
            }

            _packs[cardPack] = cards;
        }
    }
}
