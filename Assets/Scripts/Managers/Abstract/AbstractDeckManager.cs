using System;
using System.Collections.Generic;

using UnityEngine;

using InterruptingCards.Behaviours;
using InterruptingCards.Models;
using InterruptingCards.Factories;

namespace InterruptingCards.Managers
{
    public abstract class AbstractDeckManager<S, R> : MonoBehaviour, IDeckManager<S, R> where S : Enum where R : Enum
    {
        [SerializeField] protected ICardBehaviour<S, R> _topCard;

        protected bool _isFaceUp = false;
        protected IDeck<S, R> _deck;
        protected IDeck<S, R> _deckPrototype;

        public event Action OnDeckClicked;

        protected abstract ICardFactory<S, R> CardFactory { get; }

        protected abstract IDeckFactory<S, R> DeckFactory { get; }

        public virtual bool IsFaceUp
        {
            get { return _isFaceUp; }
            set
            {
                _isFaceUp = value;
                Refresh();
            }
        }

        public virtual IDeck<S, R> Deck
        {
            get { return _deck; }
            set
            {
                _deck = value;
                Refresh();
            }
        }

        public virtual object Clone()
        {
            throw new NotImplementedException("DeckManager should not be cloned");
        }

        public virtual int Count()
        {
            return _deck.Count();
        }

        public virtual void Shuffle()
        {
            _deck.Shuffle();
            Refresh();
        }
        public virtual void PlaceTop(ICard<S, R> card)
        {
            _deck.PlaceTop(card);
            Refresh();
        }

        public virtual void PlaceBottom(ICard<S, R> card)
        {
            _deck.PlaceBottom(card);
            Refresh();
        }

        public virtual void InsertRandom(ICard<S, R> card)
        {
            _deck.InsertRandom(card);
            Refresh();
        }

        public virtual ICard<S, R> PeekTop()
        {
            return _deck.PeekTop();
        }

        public virtual ICard<S, R> DrawTop()
        {
            var card = _deck.DrawTop();
            Refresh();
            return card;
        }

        public virtual ICard<S, R> DrawBottom()
        {
            var card = _deck.DrawBottom();
            Refresh();
            return card;
        }

        public virtual ICard<S, R> Remove(S suit, R rank)
        {
            var card = _deck.Remove(suit, rank);
            Refresh();
            return card;
        }

        public virtual void ResetDeck()
        {
            _deck = DeckFactory.Clone(_deckPrototype);
            Shuffle();
        }

        protected virtual void Awake()
        {
            CreatePrototype();
        }

        protected virtual void OnEnable()
        {
            _topCard.OnCardClicked -= InvokeOnDeckClicked;
            _topCard.OnCardClicked += InvokeOnDeckClicked;
        }

        protected virtual void OnDisable()
        {
            _topCard.OnCardClicked -= InvokeOnDeckClicked;
        }

        protected virtual void CreatePrototype()
        {
            var suitValues = Enum.GetValues(typeof(S));
            var rankValues = Enum.GetValues(typeof(R));
            var cards = new List<ICard<S, R>>(suitValues.Length * rankValues.Length);

            foreach (var suit in suitValues)
            {
                foreach (var rank in rankValues)
                {
                    cards.Add(CardFactory.Create((S)suit, (R)rank));
                }
            }

            _deckPrototype = DeckFactory.Create(cards);
        }

        protected virtual void Refresh()
        {
            _topCard.IsFaceUp = IsFaceUp;
            _topCard.Card = _deck.Count() == 0 ? null : PeekTop();
        }

        protected virtual void InvokeOnDeckClicked()
        {
            OnDeckClicked.Invoke();
        }
    }
}
