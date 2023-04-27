using System.Collections.Generic;

namespace InterruptingCards.Models
{
    public abstract class AbstractHand : IHand
    {
        protected readonly IList<ICard> _cards;

        protected AbstractHand(IList<ICard> cards)
        {
            _cards = cards;
        }

        public virtual int Count()
        {
            return _cards.Count;
        }

        public virtual void Insert(int i, ICard card)
        {
            _cards.Insert(i, card);
        }

        public virtual void Add(ICard card)
        {
            Insert(0, card);
        }

        public virtual ICard Remove(SuitEnum suit, RankEnum rank)
        {
            return Utilities.Remove(_cards, suit, rank);
        }

        public virtual ICard Get(int i)
        {
            return _cards[i];
        }

        public abstract object Clone();
    }
}