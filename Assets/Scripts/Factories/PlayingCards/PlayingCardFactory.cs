using UnityEngine;

using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    // TODO: Are these factories still necessary?
    public class PlayingCardFactory : MonoBehaviour, ICardFactory
    {
        public static ICardFactory Singleton { get; private set; }

        public ICard Create(SuitEnum suit, RankEnum rank)
        {
            return new PlayingCard(suit, rank);
        }

        public ICard Clone(ICard original)
        {
            return (PlayingCard)original.Clone();
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
