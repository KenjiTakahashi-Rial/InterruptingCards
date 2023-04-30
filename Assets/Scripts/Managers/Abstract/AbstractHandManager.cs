using System;
using System.Collections.Generic;

using UnityEngine;

using InterruptingCards.Behaviours;
using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public abstract class AbstractHandManager : MonoBehaviour, IHandManager
    {
        public event Action<ICard> OnCardClicked;

        public virtual IHand Hand { get; set; }

        public virtual int Count => Hand == null ? 0 : Hand.Count;

        protected abstract IList<ICardBehaviour> CardSlots { get; }

        public virtual void Add(ICard card)
        {
            Debug.Log($"Adding card to hand ({card})");

            if (Count == CardSlots.Count)
            {
                throw new TooManyCardsException();
            }

            Hand.Add(card);
            SetSlotCard(Count - 1);
        }

        public virtual ICard Remove(SuitEnum suit, RankEnum rank)
        {
            Debug.Log($"Removing {rank} | {suit} from hand");

            var card = Hand.Remove(suit, rank);
            SetAllSlotCards();
            return card;
        }

        public virtual ICard Get(int i)
        {
            return Hand.Get(i);
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
            for (var i = 0; i < CardSlots.Count; i++)
            {
                var j = i;
                void HandleValueChanged() => RefreshCard(j);
                CardSlots[i].OnValueChanged -= HandleValueChanged;
                CardSlots[i].OnValueChanged += HandleValueChanged;
            }

            SetAllSlotCards();
        }

        protected virtual void OnDisable()
        {
            foreach (var cardSlot in CardSlots)
            {
                cardSlot.UnsubscribeAllOnClicked();
                cardSlot.UnsubscribeAllOnValueChanged();
                cardSlot.Card = null;
            }
        }

        protected virtual void RefreshCard(int i)
        {
            var cardSlot = CardSlots[i];
            cardSlot.UnsubscribeAllOnClicked();

            if (cardSlot.Card != null)
            {
                cardSlot.OnClicked += () => OnCardClicked.Invoke(cardSlot.Card);
            }
        }

        protected virtual void SetSlotCard(int i)
        {
            CardSlots[i].Card = i >= Count ? null : Get(i); 
        }

        protected virtual void SetAllSlotCards()
        {
            for (var i = 0; i < CardSlots.Count; i++)
            {
                SetSlotCard(i);
            }
        }
    }
}
