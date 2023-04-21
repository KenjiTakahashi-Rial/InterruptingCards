using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public class HandManager<S, R> : MonoBehaviour, IHand<S, R> where S : Enum where R : Enum
    {
        [SerializeField] private IList<CardBehaviour<S, R>> _cardSlots;

        private IHand<S, R> _hand;

        public event Action<ICard<S, R>> OnCardClicked;

        public IHand<S, R> Hand
        {
            get { return _hand; }
            set
            {
                _hand = value;
                Refresh();
            }
        }

        public int Count()
        {
            return _hand.Count();
        }

        public void Add(ICard<S, R> card)
        {
            if (Count() == _cardSlots.Count)
            {
                throw new TooManyCardsException();
            }

            _hand.Add(card);
            _cardSlots[Count() - 1].Card = card;

            Refresh();
        }

        public ICard<S, R> Remove(S suit, R rank)
        {
            var card = _hand.Remove(suit, rank);
            _cardSlots.Remove(_cardSlots.FirstOrDefault(c => c.Card.Suit.Equals(suit) && c.Card.Rank.Equals(rank)));
            Refresh();
            return card;
        }

        public ICard<S, R> Get(int i)
        {
            return _hand.Get(i);
        }

        public void SetIsFaceUp(int i)
        {
            _cardSlots[i].IsFaceUp = true;
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void OnDisable()
        {
            foreach (var cardSlot in _cardSlots)
            {
                cardSlot.UnsubscribeAllOnCardClicked();
                cardSlot.Card = null;
            }
        }

        private void Refresh()
        {
            for (var i = 0; i < _cardSlots.Count; i++)
            {
                var cardSlot = _cardSlots[i];
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
