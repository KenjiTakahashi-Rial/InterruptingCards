using System.Collections.Generic;

using UnityEngine;

using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public class BasicHandFactory : MonoBehaviour, IHandFactory
    {
        public static IHandFactory Singleton { get; private set; }

        public IHand Create(IList<ICard> cards = null)
        {
            return new BasicHand(cards ?? new List<ICard>());
        }

        public IHand Clone(IHand original)
        {
            return (BasicHand)original.Clone();
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
