using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public class BasicCardFactory : ICardFactory<BasicCard>
    {
        protected static readonly ICardFactory<BasicCard> Instance = new BasicCardFactory();
        
        protected readonly HashSet<CardPack> _packs = new();
        protected readonly Dictionary<int, BasicCard> _cards = new();

        private BasicCardFactory() { }

        public static ICardFactory<BasicCard> Singleton { get { return Instance; } }

        public virtual BasicCard Create(int id)
        {
            if (!_cards.ContainsKey(id))
            {
                Debug.LogError($"Factory does not contain {id}. Is the card pack loaded?");
                return null;
            }

            // Don't need to copy since the card is read-only
            return _cards[id];
        }

        public virtual void Load(CardPack cardPack)
        {
            if (_packs.Contains(cardPack))
            {
                return;
            }

            var pack = CardConfig.Singleton.GetCardPack(cardPack);
            var cards = pack.ToDictionary(c => c.Id, c => new BasicCard(c));

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
    }
}
