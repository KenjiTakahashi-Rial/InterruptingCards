using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using InterruptingCards.Behaviours;
using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public abstract class AbstractHandManager<S, R> : MonoBehaviour, IHandManager<S, R> where S : Enum where R : Enum
    {
        protected IHand<S, R> _hand;

        public event Action<ICard<S, R>> OnCardClicked;

        public virtual IHand<S, R> Hand
        {
            get => _hand;
            set
            {
                _hand = value;
                Refresh();
            }
        }

        public virtual int Count()
        {
            return _hand.Count();
        }

        protected abstract IList<ICardBehaviour<S, R>> CardSlots { get; }

        public virtual void Add(ICard<S, R> card)
        {
            if (Count() == CardSlots.Count)
            {
                throw new TooManyCardsException();
            }

            _hand.Add(card);
            CardSlots[Count() - 1].Card = card;

            Refresh();
        }

        public virtual ICard<S, R> Remove(S suit, R rank)
        {
            var card = _hand.Remove(suit, rank);
            CardSlots.Remove(CardSlots.FirstOrDefault(c => c.Card.Suit.Equals(suit) && c.Card.Rank.Equals(rank)));
            Refresh();
            return card;
        }

        public virtual ICard<S, R> Get(int i)
        {
            return _hand.Get(i);
        }

        public virtual void SetIsFaceUp(int i)
        {
            CardSlots[i].IsFaceUp = true;
        }

        public virtual object Clone()
        {
            throw new NotImplementedException("HandManager should not be cloned");
        }

        protected virtual void OnEnable()
        {
            Refresh();
        }

        protected virtual void OnDisable()
        {
            foreach (var cardSlot in CardSlots)
            {
                cardSlot.UnsubscribeAllOnCardClicked();
                cardSlot.Card = null;
            }
        }

        protected virtual void Refresh()
        {
            for (var i = 0; i < CardSlots.Count; i++)
            {
                var cardSlot = CardSlots[i];
                cardSlot.UnsubscribeAllOnCardClicked();

                if (i >= Count())
                {
                    cardSlot.Card = null;
                    continue;
                }

                var card = Get(i);
                cardSlot.Card = card;
                cardSlot.OnCardClicked += () => OnCardClicked.Invoke(card);
            }
        }
    }
}
