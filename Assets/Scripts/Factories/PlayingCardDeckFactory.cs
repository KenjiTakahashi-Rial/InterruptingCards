using System.Collections.Generic;

using UnityEngine;

using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public class PlayingCardDeckFactory : MonoBehaviour, IDeckFactory
    {
        public static IDeckFactory Singleton { get; private set; }

        public IDeck Create(IList<ICard> cards)
        {
            return new PlayingCardDeck(cards);
        }

        public IDeck Clone(IDeck original)
        {
            return (PlayingCardDeck)original.Clone();
        }

        private void Awake()
        {
            Singleton = this;
        }

        private void OnDestroy()
        {
            Singleton = null;
        }
    }
}
