using System.Collections.Generic;

using UnityEngine;

using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public class PlayingCardHandFactory : MonoBehaviour, IHandFactory<PlayingCardSuit, PlayingCardRank>
    {
        public static IHandFactory<PlayingCardSuit, PlayingCardRank> Singleton { get; private set; }

        public IHand<PlayingCardSuit, PlayingCardRank> Create(IList<ICard<PlayingCardSuit, PlayingCardRank>> cards)
        {
            return new PlayingCardHand(cards);
        }

        public IHand<PlayingCardSuit, PlayingCardRank> Clone(IHand<PlayingCardSuit, PlayingCardRank> original)
        {
            return (PlayingCardHand)original.Clone();
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
