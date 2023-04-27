using System.Collections.Generic;

using UnityEngine;

using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public class PlayingCardHandFactory : MonoBehaviour, IHandFactory
    {
        public static IHandFactory Singleton { get; private set; }

        public IHand Create(IList<ICard> cards)
        {
            return new PlayingCardHand(cards);
        }

        public IHand Clone(IHand original)
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
