using System.Collections.Generic;

namespace InterruptingCards.Models
{
    public class BasicHand : IHand
    {
        protected readonly IList<ICard> _cards;

        public BasicHand(IList<ICard> cards)
        {
            _cards = cards;
        }

        public virtual int Count => _cards.Count;

        public virtual void Insert(int i, ICard card)
        {
            _cards.Insert(i, card);
        }

        public virtual void Add(ICard card)
        {
            _cards.Add(card);
        }

        public virtual ICard Remove(int cardId)
        {
            return Utilities.Remove(_cards, cardId);
        }

        public virtual ICard Get(int i)
        {
            return _cards[i];
        }

        public virtual void Clear()
        {
            _cards.Clear();
        }

        public virtual object Clone()
        {
            return new BasicHand(_cards);
        }
    }
}