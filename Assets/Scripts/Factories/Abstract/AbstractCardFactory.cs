using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public abstract class AbstractCardFactory<C> : ICardFactory<C> where C : class, ICard
    {
        protected readonly CardConfig _cardConfig = CardConfig.Singleton;
        protected readonly HashSet<CardPack> _packs = new();
        protected readonly Dictionary<int, C> _cards = new();

        public virtual C Create(int id)
        {
            if (!_cards.ContainsKey(id))
            {
                Debug.LogWarning($"Factory does not contain {_cardConfig.GetCardString(id)}. Is the card pack loaded?");
                return null;
            }

            // Don't need to copy since the card should be read-only
            return _cards[id];
        }

        public virtual void Load(CardPack cardPack)
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

        protected abstract C FromMetadata(MetadataCard metadataCard);
    }
}
