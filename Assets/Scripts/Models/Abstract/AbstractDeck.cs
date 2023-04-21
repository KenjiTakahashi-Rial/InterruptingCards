using System;
using System.Collections.Generic;

using UnityEngine;

namespace InterruptingCards.Models
{
    public class AbstractDeck<S, R> : IDeck<S, R> where S : Enum where R : Enum
    {
        private readonly System.Random _random = new();
        private readonly IList<ICard<S, R>> _cards;

        private int TopIndex
        {
            get { return _cards.Count - 1; }
        }

        private int BottomIndex { get; } = 0;

        public AbstractDeck(IList<ICard<S, R>> cards = null)
        {
            if (cards == null)
            {
                return;
            }

            _cards = cards;
        }

        public int Count()
        {
            return _cards.Count;
        }

        public void Shuffle()
        {
            CheckEmpty();

            for (var i = 0; i < _cards.Count - 1; i++)
            {
                var j = _random.Next(0, _cards.Count);
                (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
            }
        }

        public void PlaceTop(ICard<S, R> card)
        {
            _cards.Add(card);
        }

        public void PlaceBottom(ICard<S, R> card)
        {
            _cards.Insert(0, card);
        }

        public void InsertRandom(ICard<S, R> card)
        {
            var i = _random.Next(0, _cards.Count + 1);
            _cards.Insert(i, card);
        }

        public ICard<S, R> PeekTop()
        {
            CheckEmpty();
            return _cards[TopIndex].Clone();
        }

        public ICard<S, R> DrawTop()
        {
            return PopAt(TopIndex);
        }

        public ICard<S, R> DrawBottom()
        {
            return PopAt(BottomIndex);
        }

        public ICard<S, R> Remove(S suit, R rank)
        {
            return Utilities.Remove(_cards, suit, rank);
        }

        private void CheckEmpty()
        {
            if (_cards.Count == 0)
            {
                throw new CardCollectionEmptyException();
            }
        }

        private ICard<S, R> PopAt(int i)
        {
            CheckEmpty();
            var card = _cards[i];
            _cards.RemoveAt(i);
            return card;
        }
    }
}