using System.Collections.Generic;

using InterruptingCards.Utilities;

namespace InterruptingCards.Models
{
    public class BasicHand : IHand<BasicCard>
    {
        protected readonly IList<BasicCard> _cards;

        // Do not call directly; use a factory
        public BasicHand(IList<BasicCard> cards)
        {
            _cards = cards;
        }

        public virtual int Count => _cards.Count;

        public virtual void Insert(int i, BasicCard card)
        {
            _cards.Insert(i, card);
        }

        public virtual void Add(BasicCard card)
        {
            _cards.Add(card);
        }

        public virtual BasicCard Remove(int cardId)
        {
            return HelperMethods.Remove(_cards, cardId);
        }

        public virtual BasicCard Get(int i)
        {
            return _cards[i];
        }

        public virtual void Clear()
        {
            _cards.Clear();
        }
    }
}