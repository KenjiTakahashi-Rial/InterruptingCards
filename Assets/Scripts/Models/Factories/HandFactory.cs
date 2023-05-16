using System.Collections.Generic;

using InterruptingCards.Models;

namespace InterruptingCards.Models
{
    public class HandFactory
    {
        private HandFactory() { }

        public static HandFactory Singleton { get; } = new();

        public Hand Create(IList<Card> cards = null)
        {
            return cards == null ? new Hand(new List<Card>()) : new Hand(cards);
        }
    }
}
