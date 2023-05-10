using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using InterruptingCards.Config;

namespace InterruptingCards.Models
{
    public class BasicFactory : MonoBehaviour, IFactory
    {
        [SerializeField] private CardPack _cardPack;

        private ImmutableDictionary<int, BasicCard> _cards;

        public static IFactory Singleton { get; private set; }

        public IPlayer CreatePlayer(ulong id, string name, IHand hand = null)
        {
            return new BasicPlayer(id, name, hand);
        }

        public ICard CreateCard(int id)
        {
            if (!_cards.ContainsKey(id))
            {
                Debug.LogError($"{_cardPack} does not contain ID {id}");
                return null;
            }

            // Don't need to copy since the card is read-only
            return _cards[id];
        }

        public IDeck CreateDeck(IList<ICard> cards = null)
        {
            return cards == null ? new BasicDeck(new List<ICard>()) : new BasicDeck(cards);
        }

        public IDeck CreateFullDeck()
        {
            var cards = new List<ICard>();
            
            foreach ((var id, var card) in _cards)
            {
                for (var i = 0; i < CardConfig.GetMetadataCard(id).Count; i++)
                {
                    cards.Add(card);
                }
            }

            return new BasicDeck(cards);
        }

        public IHand CreateHand(IList<ICard> cards = null)
        {
            return cards == null ? new BasicHand(new List<ICard>()) : new BasicHand(cards);
        }

        private void Awake()
        {
            var packMetadata = CardConfig.GetPackMetadata(_cardPack);
            var cards = packMetadata.ToDictionary(c => c.Id, c => new BasicCard(c));
            _cards = new ImmutableDictionary<int, BasicCard>(cards);

            Singleton = this;
        }

        private void OnDestroy()
        {
            Singleton = null;
        }
    }
}
