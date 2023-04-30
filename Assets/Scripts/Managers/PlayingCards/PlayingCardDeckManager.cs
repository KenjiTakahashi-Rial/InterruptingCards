using UnityEngine;

using InterruptingCards.Behaviours;
using InterruptingCards.Factories;

namespace InterruptingCards.Managers
{
    public class PlayingCardDeckManager : AbstractDeckManager
    {
        [SerializeField] private PlayingCardBehaviour _topCard;

        protected override ICardFactory CardFactory => PlayingCardFactory.Singleton;

        protected override IDeckFactory DeckFactory => PlayingCardDeckFactory.Singleton;

        protected override ICardBehaviour TopCard => _topCard;
    }
}
