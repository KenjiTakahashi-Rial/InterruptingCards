using UnityEngine;

using InterruptingCards.Models;

namespace InterruptingCards.Factories
{
    public class BasicPlayerFactory : MonoBehaviour, IPlayerFactory
    {
        public static IPlayerFactory Singleton { get; private set; }

        public virtual IPlayer Create(ulong id, string name, IHand hand = null)
        {
            return new BasicPlayer(id, name, hand);
        }

        protected virtual void Awake()
        {
            Singleton = this;
        }

        protected virtual void OnDestroy()
        {
            Singleton = null;
        }
    }
}
