using System;

using InterruptingCards.Factories;
using InterruptingCards.Models;

namespace InterruptingCards.Managers.GameManagers
{
    public class PlayingCardDeckManager : AbstractDeckManager<PlayingCardSuit, PlayingCardRank>
    {
        protected override ICardFactory<PlayingCardSuit, PlayingCardRank> CardFactory
        {
            get { return PlayingCardFactory.Singleton; }
        }

        protected override IDeckFactory<PlayingCardSuit, PlayingCardRank> DeckFactory
        {
            get { return PlayingCardDeckFactory.Singleton; }
        }
    }
}
