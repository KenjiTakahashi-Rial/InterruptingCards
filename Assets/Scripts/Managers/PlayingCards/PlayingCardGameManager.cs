using System;

using UnityEngine;

using InterruptingCards.Factories;
using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public class PlayingCardGameManager : AbstractGameManager
    {
        [SerializeField] private PlayingCardDeckManager _deckManager;
        [SerializeField] private PlayingCardDeckManager _discardManager;
        [SerializeField] private PlayingCardHandManager[] _handManagers;

        protected override IPlayerFactory PlayerFactory => PlayingCardPlayerFactory.Singleton;

        protected override ICardFactory CardFactory => PlayingCardFactory.Singleton;

        protected override IHandFactory HandFactory => PlayingCardHandFactory.Singleton;

        protected override IDeckManager DeckManager => _deckManager;

        protected override IDeckManager DiscardManager => _discardManager;

        protected override IHandManager[] HandManagers => _handManagers;

        protected override int MinPlayers => 2;

        protected override int MaxPlayers => 2;

        protected override int StartingHandCardCount => 4;

        protected override void HandleEffect(Effect effect)
        {
            switch (effect)
            {
                case Effect.PlayCard:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
