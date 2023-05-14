using System.Collections.Generic;
using System.Linq;

using InterruptingCards.Models;
using InterruptingCards.Config;

namespace InterruptingCards.Factories
{
    public class BasicDeckFactory : IDeckFactory<BasicCard, BasicDeck>
    {
        protected static readonly IDeckFactory<BasicCard, BasicDeck> Instance = new BasicDeckFactory();
        protected readonly ICardFactory<BasicCard> _cardFactory = BasicCardFactory.Singleton;
        protected readonly CardConfig _cardConfig = CardConfig.Singleton;
        protected readonly Dictionary<CardPack, IList<BasicCard>> _packs = new();

        private BasicDeckFactory() { }

        public static IDeckFactory<BasicCard, BasicDeck> Singleton { get { return Instance; } }

        public BasicDeck Create(IList<BasicCard> cards = null)
        {
            return cards == null ? new BasicDeck(new List<BasicCard>()) : new BasicDeck(cards);
        }

        public BasicDeck Create(CardPack cardPack)
        {
            if (!_packs.ContainsKey(cardPack))
            {
                Load(cardPack);
            }

            return Create(_packs[cardPack]);
        }

        protected void Load(CardPack cardPack)
        {
            _cardFactory.Load(cardPack);

            var cards = new List<BasicCard>();
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
