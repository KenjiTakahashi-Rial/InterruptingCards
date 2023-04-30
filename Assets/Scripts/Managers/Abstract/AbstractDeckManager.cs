using System;
using System.Collections.Generic;

using UnityEngine;

using InterruptingCards.Behaviours;
using InterruptingCards.Factories;
using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public abstract class AbstractDeckManager : MonoBehaviour, IDeckManager
    {
        protected IDeck _deck;

        public event Action OnDeckClicked;

        protected abstract ICardFactory CardFactory { get; }

        protected abstract IDeckFactory DeckFactory { get; }
        
        public virtual bool IsFaceUp
        {
            get => TopCard.IsFaceUp;
            set => TopCard.IsFaceUp = value;
        }

        public virtual int Count => _deck.Count;

        protected abstract ICardBehaviour TopCard { get; }

        public virtual object Clone()
        {
            throw new NotImplementedException("DeckManager should not be cloned");
        }

        public virtual void Shuffle()
        {
            _deck.Shuffle();
            SetTopCard();
        }
        public virtual void PlaceTop(ICard card)
        {
            _deck.PlaceTop(card);
            SetTopCard();
        }

        public virtual void PlaceBottom(ICard card)
        {
            _deck.PlaceBottom(card);
            SetTopCard();
        }

        public virtual void InsertRandom(ICard card)
        {
            _deck.InsertRandom(card);
            SetTopCard();
        }

        public virtual ICard PeekTop()
        {
            return _deck.PeekTop();
        }

        public virtual ICard DrawTop()
        {
            var card = _deck.DrawTop();
            SetTopCard();
            return card;
        }

        public virtual ICard DrawBottom()
        {
            var card = _deck.DrawBottom();
            SetTopCard();
            return card;
        }

        public virtual ICard Remove(SuitEnum suit, RankEnum rank)
        {
            var card = _deck.Remove(suit, rank);
            SetTopCard();
            return card;
        }

        public virtual void Initialize()
        {
            _deck = DeckFactory.Prototype;
            Shuffle();
            SetTopCard();
        }

        public virtual void Clear()
        {
            _deck = DeckFactory.Create(new List<ICard>());
            SetTopCard();
        }

        protected virtual void OnEnable()
        {
            TopCard.OnClicked -= InvokeOnDeckClicked;
            TopCard.OnClicked += InvokeOnDeckClicked;
        }

        protected virtual void OnDisable()
        {
            TopCard.OnClicked -= InvokeOnDeckClicked;
        }

        protected virtual void SetTopCard()
        {
            TopCard.Card = _deck.Count == 0 ? null : PeekTop();
        }

        protected virtual void InvokeOnDeckClicked()
        {
            Debug.Log("Deck clicked");

            OnDeckClicked.Invoke();
        }
    }
}
