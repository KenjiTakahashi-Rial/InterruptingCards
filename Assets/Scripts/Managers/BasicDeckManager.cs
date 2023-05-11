using System;

using UnityEngine;

using InterruptingCards.Behaviours;
using InterruptingCards.Config;
using InterruptingCards.Factories;
using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public class BasicDeckManager : MonoBehaviour, IDeckManager<BasicCard>
    {
        [SerializeField] protected BasicCardBehaviour _topCard;

        protected BasicDeck _deck;

        public event Action OnDeckClicked;

        protected virtual IDeckFactory<BasicCard, BasicDeck> DeckFactory => BasicDeckFactory.Singleton;
        
        public virtual bool IsFaceUp
        {
            get => _topCard.IsFaceUp;
            set => _topCard.IsFaceUp = value;
        }

        public virtual int Count => _deck.Count;

        public virtual void Shuffle()
        {
            _deck.Shuffle();
            SetTopCard();
        }
        public virtual void PlaceTop(BasicCard card)
        {
            _deck.PlaceTop(card);
            SetTopCard();
        }

        public virtual void PlaceBottom(BasicCard card)
        {
            _deck.PlaceBottom(card);
            SetTopCard();
        }

        public virtual void InsertRandom(BasicCard card)
        {
            _deck.InsertRandom(card);
            SetTopCard();
        }

        public virtual BasicCard PeekTop()
        {
            return _deck.PeekTop();
        }

        public virtual BasicCard DrawTop()
        {
            var card = _deck.DrawTop();
            SetTopCard();
            return card;
        }

        public virtual BasicCard DrawBottom()
        {
            var card = _deck.DrawBottom();
            SetTopCard();
            return card;
        }

        public virtual BasicCard Remove(int cardId)
        {
            var card = _deck.Remove(cardId);
            SetTopCard();
            return card;
        }

        public virtual void Initialize(CardPack cardPack)
        {
            _deck = DeckFactory.Create(cardPack);
            SetTopCard();
        }

        public virtual void Clear()
        {
            _deck = DeckFactory.Create();
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
                Debug.Log("OnDeckClicked has no subscribers");
            }
            else
            {
                Debug.Log("Deck clicked");
            }

            OnDeckClicked?.Invoke();
        }
    }
}
