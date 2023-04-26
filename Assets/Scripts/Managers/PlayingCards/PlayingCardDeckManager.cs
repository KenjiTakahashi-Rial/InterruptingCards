using UnityEngine;

using InterruptingCards.Behaviours;
using InterruptingCards.Factories;
using InterruptingCards.Models;

namespace InterruptingCards.Managers.GameManagers
{
    public class PlayingCardDeckManager : AbstractDeckManager<PlayingCardSuit, PlayingCardRank>
    {
        [SerializeField] private PlayingCardBehaviour _topCard;

        protected override ICardFactory<PlayingCardSuit, PlayingCardRank> CardFactory
        {
            get => PlayingCardFactory.Singleton;
        }

        protected override IDeckFactory<PlayingCardSuit, PlayingCardRank> DeckFactory
        {
            get => PlayingCardDeckFactory.Singleton;
        }

        protected override ICardBehaviour<PlayingCardSuit, PlayingCardRank> TopCard { get => _topCard; }
    }
}
