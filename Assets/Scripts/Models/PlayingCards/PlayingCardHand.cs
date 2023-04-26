using System.Collections.Generic;

using InterruptingCards.Factories;

namespace InterruptingCards.Models
{
    public class PlayingCardHand : AbstractHand<PlayingCardSuit, PlayingCardRank>
    {
        public PlayingCardHand(IList<ICard<PlayingCardSuit, PlayingCardRank>> cards) : base(cards) { }

        public override object Clone()
        {
            return new PlayingCardHand(_cards);
        }
    }
}