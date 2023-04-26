using InterruptingCards.Factories;
using InterruptingCards.Models;

using UnityEngine;

namespace InterruptingCards.Managers.GameManagers
{
    public class PlayingCardGameManager : AbstractGameManager<PlayingCardSuit, PlayingCardRank>
    {
        [SerializeField] private PlayingCardDeckManager _deckManager;
        [SerializeField] private PlayingCardDeckManager _discardManager;
        [SerializeField] private PlayingCardHandManager[] _handManagers;

        protected override IGameManagerNetworkDependency<PlayingCardSuit, PlayingCardRank> NetworkDependency =>
            PlayingCardGameManagerNetworkDependency.Singleton;

        protected override IPlayerFactory<PlayingCardSuit, PlayingCardRank> PlayerFactory =>
            PlayingCardPlayerFactory.Singleton;

        protected override ICardFactory<PlayingCardSuit, PlayingCardRank> CardFactory =>
            PlayingCardFactory.Singleton;

        protected override IHandFactory<PlayingCardSuit, PlayingCardRank> HandFactory =>
            PlayingCardHandFactory.Singleton;

        protected override IDeckManager<PlayingCardSuit, PlayingCardRank> DeckManager { get => _deckManager; }

        protected override IDeckManager<PlayingCardSuit, PlayingCardRank> DiscardManager { get => _discardManager; }

        protected override IHandManager<PlayingCardSuit, PlayingCardRank>[] HandManagers { get => _handManagers; }

        protected override int MinPlayers => 2;

        protected override int MaxPlayers => 2;

        protected override int StartingHandCardCount => 5;
    }
}
