using System;
using System.Collections.Generic;

using UnityEngine;

using InterruptingCards.Behaviours;
using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public abstract class AbstractHandManager : MonoBehaviour, IHandManager
    {
        protected IHand _hand;

        public event Action<ICard> OnCardClicked;

        public virtual IHand Hand
        {
            get => _hand;
            set
            {
                _hand = value;
                Refresh();
            }
        }

        public virtual int Count => _hand == null ? 0 : _hand.Count;

        protected abstract IList<ICardBehaviour> CardSlots { get; }

        public virtual void Add(ICard card)
        {
            if (Count == CardSlots.Count)
            {
                throw new TooManyCardsException();
            }

            _hand.Add(card);
            CardSlots[Count - 1].Card = card;

            Refresh();
        }

        public virtual ICard Remove(SuitEnum suit, RankEnum rank)
        {
            var card = _hand.Remove(suit, rank);
            Refresh();
            return card;
        }

        public virtual ICard Get(int i)
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

                if (i >= Count)
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
