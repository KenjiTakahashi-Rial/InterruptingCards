using UnityEngine;

using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public class PlayingCardPlayerFactory : MonoBehaviour, IPlayerFactory
    {
        public static IPlayerFactory Singleton { get; private set; }

        public IPlayer Create(
            ulong id, string name, IHand hand = null
        )
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
