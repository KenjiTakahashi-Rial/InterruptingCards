using System;
using System.Collections.Generic;

using UnityEngine;

using InterruptingCards.Behaviours;
using InterruptingCards.Config;
using InterruptingCards.Factories;
using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public class BasicDeckManager : MonoBehaviour, IDeckManager
    {
        [SerializeField] protected BasicCardBehaviour _topCard;

        protected IDeck _deck;

        public event Action OnDeckClicked;

        protected virtual ICardFactory CardFactory => BasicCardFactory.Singleton;

        protected virtual IDeckFactory DeckFactory => BasicDeckFactory.Singleton;
        
        public virtual bool IsFaceUp
        {
            get => _topCard.IsFaceUp;
            set => _topCard.IsFaceUp = value;
        }

        public virtual int Count => _deck.Count;

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

        public virtual ICard Remove(CardSuit suit, CardRank rank)
        {
            var card = _deck.Remove(suit, rank);
            SetTopCard();
            return card;
        }

        public virtual void Initialize()
        {
            _deck = DeckFactory.Prototype;
            SetTopCard();
        }

        public virtual void Clear()
        {
            _deck = DeckFactory.Create(new List<ICard>());
            SetTopCard();
        }

        protected virtual void OnEnable()
        {
            _topCard.OnClicked -= InvokeOnDeckClicked;
            _topCard.OnClicked += InvokeOnDeckClicked;

            SetTopCard();
        }

        protected virtual void OnDisable()
        {
            _topCard.OnClicked -= InvokeOnDeckClicked;
        }

        protected virtual void SetTopCard()
        {
            _topCard.Card = _deck == null || _deck.Count == 0 ? null : PeekTop();
        }

        protected virtual void InvokeOnDeckClicked()
        {
            if (OnDeckClicked == null)
            {
                Debug.Log("Deck clicked, but OnDeckClicked is null");
            }
            else
            {
                Debug.Log("Deck clicked");
            }

            OnDeckClicked?.Invoke();
        }
    }
}
