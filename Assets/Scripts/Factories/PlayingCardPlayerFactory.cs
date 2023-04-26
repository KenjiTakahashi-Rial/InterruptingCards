using UnityEngine;

using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public class PlayingCardPlayerFactory : MonoBehaviour, IPlayerFactory<PlayingCardSuit, PlayingCardRank>
    {
        public static IPlayerFactory<PlayingCardSuit, PlayingCardRank> Singleton { get; private set; }

        public IPlayer<PlayingCardSuit, PlayingCardRank> Create(ulong id, string name, IHand<PlayingCardSuit, PlayingCardRank> hand = null)
        {
            return new PlayingCardPlayer(id, name, hand);
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
