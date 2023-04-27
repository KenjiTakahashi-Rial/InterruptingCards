using System.Collections.Generic;

using InterruptingCards.Factories;

namespace InterruptingCards.Models
{
    public class PlayingCardDeck : AbstractDeck
    {
        protected override ICardFactory CardFactory => PlayingCardFactory.Singleton;

        public PlayingCardDeck(IList<ICard> cards) : base(cards) { }

        public override object Clone()
        {
            return new PlayingCardDeck(_cards);
        }
    }
}