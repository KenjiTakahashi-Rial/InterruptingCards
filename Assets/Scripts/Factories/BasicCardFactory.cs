using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public class BasicCardFactory : MonoBehaviour, ICardFactory
    {
        public static ICardFactory Singleton { get; private set; }

        public ICard Create(CardSuit suit, CardRank rank)
        {
            return new BasicCard(suit, rank);
        }

        public ICard Clone(ICard original)
        {
            return (BasicCard)original.Clone();
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
