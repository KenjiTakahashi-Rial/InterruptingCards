using System.Collections.Generic;

using InterruptingCards.Factories;

namespace InterruptingCards.Models
{
    public class PlayingCardHand : AbstractHand
    {
        public PlayingCardHand(IList<ICard> cards) : base(cards) { }

        public override object Clone()
        {
            return new PlayingCardHand(_cards);
        }
    }
}