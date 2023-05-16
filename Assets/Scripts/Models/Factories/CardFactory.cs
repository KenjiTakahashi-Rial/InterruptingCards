using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Models;

namespace InterruptingCards.Models
{
    public class CardFactory
    {
        private readonly CardConfig _cardConfig = CardConfig.Singleton;
        private readonly HashSet<CardPack> _packs = new();
        private readonly Dictionary<int, Card> _cards = new();

        private CardFactory() { }

        public static CardFactory Singleton { get; } = new();

        public Card Create(int id)
        {
            if (!_cards.ContainsKey(id))
            {
                Debug.LogWarning($"Factory does not contain {_cardConfig.GetCardString(id)}. Is the card pack loaded?");
                return null;
            }

            // Don't need to copy since the card should be read-only
            return _cards[id];
        }

        public void Load(CardPack cardPack)
        {
            if (_packs.Contains(cardPack))
            {
                Debug.Log($"Factory already loaded {cardPack}");
                return;
            }

            Debug.Log($"Factory loading {cardPack}");

            var pack = _cardConfig.GetCardPack(cardPack);
            var cards = pack.ToDictionary(c => c.Id, c => FromMetadata(c));

            foreach ((var id, var card) in cards)
            {
                if (_cards.ContainsKey(id))
                {
                    Debug.LogWarning($"Factory already contains {id}. Overwriting {_cards[id].Name} with {card.Name}");
                }

                _cards[id] = card;
            }

            _packs.Add(cardPack);
        }

        private Card FromMetadata(MetadataCard metadataCard)
        {
            return new Card(metadataCard);
        }
    }
}
