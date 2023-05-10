using System.Collections.Generic;

using UnityEngine;

using InterruptingCards.Config;
using System;

namespace InterruptingCards.Models
{
    public class BasicFactory : MonoBehaviour, IFactory
    {
        [SerializeField] private CardPack _cardPack;

        private ImmutableDictionary<int, BasicCard> _cards;
        private ImmutableDictionary<int, PackCard> _packCards;

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
                for (var i = 0; i < _packCards[id].Count; i++)
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
            var cards = new Dictionary<int, BasicCard>();
            var packCards = new Dictionary<int, PackCard>();
            var pack = CardConfig.GetPack(_cardPack);

            foreach (var packCard in pack)
            {
                var suit = (CardSuit)Enum.Parse(typeof(CardSuit), packCard.Suit);
                var rank = (CardRank)Enum.Parse(typeof(CardRank), packCard.Rank);
                var id = CardConfig.CardId(suit, rank);

                cards[id] = new BasicCard(id, suit, rank, packCard.Name);
                packCards[id] = packCard;
            }

            _cards = new ImmutableDictionary<int, BasicCard>(cards);
            _packCards = new ImmutableDictionary<int, PackCard>(packCards);

            Singleton = this;
        }

        private void OnDestroy()
        {
            Singleton = null;
        }
    }
}
