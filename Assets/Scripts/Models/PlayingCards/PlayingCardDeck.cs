using System.Collections.Generic;

using InterruptingCards.Factories;

namespace InterruptingCards.Models
{
    public class PlayingCardDeck : AbstractDeck<PlayingCardSuit, PlayingCardRank>
    {
        protected override ICardFactory<PlayingCardSuit, PlayingCardRank> CardFactory =>
            PlayingCardFactory.Singleton;

        public PlayingCardDeck(IList<ICard<PlayingCardSuit, PlayingCardRank>> cards) : base(cards) { }

        public override object Clone()
        {
            return new PlayingCardDeck(_cards);
        }
    }
}