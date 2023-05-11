using System;
using System.Collections.Generic;

using UnityEngine;

using InterruptingCards.Behaviours;
using InterruptingCards.Factories;
using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public class BasicHandManager : MonoBehaviour, IHandManager<BasicCard, BasicHand>
    {
        [SerializeField] private List<BasicCardBehaviour> _cardSlots;

        public event Action<BasicCard> OnCardClicked;

        public virtual BasicHand Hand { get; private set; }

        public virtual int Count => Hand.Count;

        public virtual void Add(BasicCard card)
        {
            Debug.Log($"Adding card to hand ({card})");

            if (Count == _cardSlots.Count)
            {
                throw new TooManyCardsException();
            }

            Hand.Add(card);
            SetSlotCard(Count - 1);
        }

        public virtual BasicCard Remove(int cardId)
        {
            Debug.Log($"Removing {cardId} from hand");

            var card = Hand.Remove(cardId);
            SetAllSlotCards();
            return card;
        }

        public virtual BasicCard Get(int i)
        {
            return Hand.Get(i);
        }

        public virtual void Clear()
        {
            Hand.Clear();
            SetAllSlotCards();
        }

        public virtual void SetIsFaceUp(int i)
        {
            _cardSlots[i].IsFaceUp = true;
        }

        protected virtual void OnEnable()
        {
            Hand = BasicHandFactory.Singleton.Create();

            for (var i = 0; i < _cardSlots.Count; i++)
            {
                var j = i;
                void HandleValueChanged() => RefreshCard(j);
                _cardSlots[i].OnValueChanged -= HandleValueChanged;
                _cardSlots[i].OnValueChanged += HandleValueChanged;
            }

            SetAllSlotCards();
        }

        protected virtual void OnDisable()
        {
            foreach (var cardSlot in _cardSlots)
            {
                cardSlot.UnsubscribeAllOnClicked();
                cardSlot.UnsubscribeAllOnValueChanged();
                cardSlot.Card = null;
            }
        }

        protected virtual void RefreshCard(int i)
        {
            var cardSlot = _cardSlots[i];
            cardSlot.UnsubscribeAllOnClicked();

            if (cardSlot.Card != null)
            {
                cardSlot.OnClicked += () => OnCardClicked.Invoke(cardSlot.Card);
            }
        }

        protected virtual void SetSlotCard(int i)
        {
            _cardSlots[i].Card = i >= Count ? null : Get(i);
        }

        protected virtual void SetAllSlotCards()
        {
            for (var i = 0; i < _cardSlots.Count; i++)
            {
                SetSlotCard(i);
            }
        }
    }
}
