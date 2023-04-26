using UnityEngine;

using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public class PlayingCardFactory : MonoBehaviour, ICardFactory<PlayingCardSuit, PlayingCardRank>
    {
        public static ICardFactory<PlayingCardSuit, PlayingCardRank> Singleton { get; private set; }

        public ICard<PlayingCardSuit, PlayingCardRank> Create(PlayingCardSuit suit, PlayingCardRank rank)
        {
            return new PlayingCard(suit, rank);
        }

        public ICard<PlayingCardSuit, PlayingCardRank> Clone(ICard<PlayingCardSuit, PlayingCardRank> original)
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
