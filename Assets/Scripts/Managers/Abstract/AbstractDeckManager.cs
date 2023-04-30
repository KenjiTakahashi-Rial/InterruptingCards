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
        protected IDeck _deckPrototype;

        public event Action OnDeckClicked;

        protected abstract ICollection<SuitEnum> Suits { get; }

        protected abstract ICollection<RankEnum> Ranks { get; }

        protected abstract ICardFactory CardFactory { get; }

        protected abstract IDeckFactory DeckFactory { get; }
        
        public virtual bool IsFaceUp
        {
            get => TopCard.IsFaceUp;
            set => TopCard.IsFaceUp = value;
        }

        protected abstract ICardBehaviour TopCard { get; }

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
        public virtual void PlaceTop(ICard card)
        {
            _deck.PlaceTop(card);
            Refresh();
        }

        public virtual void PlaceBottom(ICard card)
        {
            _deck.PlaceBottom(card);
            Refresh();
        }

        public virtual void InsertRandom(ICard card)
        {
            _deck.InsertRandom(card);
            Refresh();
        }

        public virtual ICard PeekTop()
        {
            return _deck.PeekTop();
        }

        public virtual ICard DrawTop()
        {
            var card = _deck.DrawTop();
            Refresh();
            return card;
        }

        public virtual ICard DrawBottom()
        {
            var card = _deck.DrawBottom();
            Refresh();
            return card;
        }

        public virtual ICard Remove(SuitEnum suit, RankEnum rank)
        {
            var card = _deck.Remove(suit, rank);
            Refresh();
            return card;
        }

        public virtual void Initialize()
        {
            _deck = DeckFactory.Clone(_deckPrototype);
            Shuffle();
            Refresh();
        }

        public virtual void Clear()
        {
            _deck = DeckFactory.Create(new List<ICard>());
            Refresh();
        }

        protected virtual void Start()
        {
            // Must be called after awake so Singletons can be set
            CreatePrototype();
        }

        protected virtual void OnEnable()
        {
            TopCard.OnCardClicked -= InvokeOnDeckClicked;
            TopCard.OnCardClicked += InvokeOnDeckClicked;
        }

        protected virtual void OnDisable()
        {
            TopCard.OnCardClicked -= InvokeOnDeckClicked;
        }

        protected virtual void CreatePrototype()
        {
            var cards = new List<ICard>(Suits.Count * Ranks.Count);

            foreach (var suit in Suits)
            {
                foreach (var rank in Ranks)
                {
                    cards.Add(CardFactory.Create(suit, rank));
                }
            }

            _deckPrototype = DeckFactory.Create(cards);
        }

        protected virtual void Refresh()
        {
            TopCard.Card = _deck.Count() == 0 ? null : PeekTop();
        }

        protected virtual void InvokeOnDeckClicked()
        {
            Debug.Log("Deck clicked");
            OnDeckClicked.Invoke();
        }
    }
}
