using System.Collections.Generic;

using UnityEngine;

using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public class PlayingCardDeckFactory : MonoBehaviour, IDeckFactory<PlayingCardSuit, PlayingCardRank>
    {
        public static IDeckFactory<PlayingCardSuit, PlayingCardRank> Singleton { get; private set; }

        public IDeck<PlayingCardSuit, PlayingCardRank> Create(IList<ICard<PlayingCardSuit, PlayingCardRank>> cards)
        {
            return new PlayingCardDeck(cards);
        }

        public IDeck<PlayingCardSuit, PlayingCardRank> Clone(IDeck<PlayingCardSuit, PlayingCardRank> original)
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
