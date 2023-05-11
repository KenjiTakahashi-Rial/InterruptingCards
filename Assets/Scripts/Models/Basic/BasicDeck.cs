using System;
using System.Collections.Generic;

using InterruptingCards.Factories;
using InterruptingCards.Utilities;

namespace InterruptingCards.Models
{
    public class BasicDeck : IDeck<BasicCard>
    {
        protected readonly Random _random = new();
        protected IList<BasicCard> _cards;

        public virtual int Count => _cards.Count;

        protected virtual ICardFactory<BasicCard> CardFactory => BasicCardFactory.Singleton;

        protected virtual int TopIndex { get => _cards.Count - 1;}

        protected virtual int BottomIndex { get; } = 0;

        // Do not call directly; use a factory
        public BasicDeck(IList<BasicCard> cards)
        {
            _cards = cards;
        }

        public virtual void Shuffle()
        {
            CheckEmpty();

            for (var i = 0; i < _cards.Count - 1; i++)
            {
                var j = _random.Next(0, _cards.Count);
                (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
            }
        }

        public virtual void PlaceTop(BasicCard card)
        {
            _cards.Add(card);
        }

        public virtual void PlaceBottom(BasicCard card)
        {
            _cards.Insert(0, card);
        }

        public virtual void InsertRandom(BasicCard card)
        {
            var i = _random.Next(0, _cards.Count + 1);
            _cards.Insert(i, card);
        }

        public virtual BasicCard PeekTop()
        {
            CheckEmpty();
            return CardFactory.Create(_cards[TopIndex].Id);
        }

        public virtual BasicCard DrawTop()
        {
            return PopAt(TopIndex);
        }

        public virtual BasicCard DrawBottom()
        {
            return PopAt(BottomIndex);
        }

        public virtual BasicCard Remove(int cardId)
        {
            return HelperMethods.Remove(_cards, cardId);
        }

        protected virtual void CheckEmpty()
        {
            if (_cards.Count == 0)
            {
                throw new CardCollectionEmptyException();
            }
        }

        protected virtual BasicCard PopAt(int i)
        {
            CheckEmpty();
            var card = _cards[i];
            _cards.RemoveAt(i);
            return card;
        }
    }
}