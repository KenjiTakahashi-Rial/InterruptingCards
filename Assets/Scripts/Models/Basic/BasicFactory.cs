using System.Collections.Generic;

using UnityEngine;

using InterruptingCards.Config;

namespace InterruptingCards.Models
{
    public class BasicFactory : MonoBehaviour, IFactory
    {
        [SerializeField] private CardPack _cardPack;

        private ImmutableDictionary<int, BasicCard> _cards;
        private ImmutableDictionary<int, int> _counts;

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
            return new BasicDeck(cards);
        }

        public IDeck CreateFullDeck()
        {
            var cards = new List<ICard>();
            
            foreach ((var id, var card) in _cards)
            {
                for (var i = 0; i < _counts[id]; i++)
                {
                    cards.Add(card);
                }
            }

            return new BasicDeck(cards);
        }

        public IHand CreateHand(IList<ICard> cards = null)
        {
            return new BasicHand(cards);
        }

        private void Awake()
        {
            var cardCounts = CardConfig.GetCardCounts<BasicCard>(_cardPack);
            _cards = cardCounts.ToDictionary(c => c.Key.Id, c => c.Key);
            _counts = cardCounts.ToDictionary(c => c.Key.Id, c => c.Value);

            Singleton = this;
        }

        private void OnDestroy()
        {
            Singleton = null;
        }
    }
}
