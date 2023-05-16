using System;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Behaviours;
using InterruptingCards.Config;
using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public class HandManager : NetworkBehaviour
    {
        private readonly HandFactory _handFactory = HandFactory.Singleton;

        [SerializeField] private CardBehaviour[] _cardSlots;

        public event Action<Card> OnCardClicked;

        public Hand Hand { get; private set; }

        private int Count => Hand == null ? 0 : Hand.Count;

        public void Add(Card card)
        {
            Debug.Log($"Adding card to hand ({card})");

            if (Count == _cardSlots.Length)
            {
                throw new TooManyCardsException("Cannot add more cards than card slots");
            }

            Hand.Add(card);
        }

        public void Clear()
        {
            Debug.Log("Clearing hand");

            Hand.Clear();
            SetCards();
        }

        private void Awake()
        {
            Hand = _handFactory.Create();
        }

        private void OnEnable()
        {
            Hand.OnChanged -= SetCards;
            Hand.OnChanged += SetCards;

            for (var i = 0; i < _cardSlots.Length; i++)
            {
                var j = i;
                void HandleCardChanged() => SetClickListener(j);
                _cardSlots[i].OnCardChanged -= HandleCardChanged;
                _cardSlots[i].OnCardChanged += HandleCardChanged;
            }

            SetAllClickListeners();
        }

        private void OnDisable()
        {
            Hand.OnChanged -= SetCards;

            foreach (var cardSlot in _cardSlots)
            {
                cardSlot.UnsubscribeAllOnClicked();
                cardSlot.UnsubscribeAllOnCardChanged();
                cardSlot.Card = null;
            }
        }

        private void SetCards()
        {
            for (var i = 0; i < _cardSlots.Length; i++)
            {
                _cardSlots[i].Card = i >= Count ? null : Hand.Get(i);
            }
        }

        private void SetClickListener(int i)
        {
            var cardSlot = _cardSlots[i];
            cardSlot.UnsubscribeAllOnClicked();

            if (cardSlot.Card != null)
            {
                cardSlot.OnClicked += () => OnCardClicked.Invoke(cardSlot.Card);
            }
        }

        private void SetAllClickListeners()
        {
            for (var i = 0; i < _cardSlots.Length; i++)
            {
                SetClickListener(i);
            }
        }
    }
}
