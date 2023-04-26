using InterruptingCards.Factories;
using InterruptingCards.Models;

namespace InterruptingCards.Managers.GameManagers
{
    public class PlayingCardGameManager : AbstractGameManager<PlayingCardSuit, PlayingCardRank>
    {
        protected override IGameManagerNetworkDependency<PlayingCardSuit, PlayingCardRank> NetworkDependency =>
            PlayingCardGameManagerNetworkDependency.Singleton;

        protected override IPlayerFactory<PlayingCardSuit, PlayingCardRank> PlayerFactory =>
            PlayingCardPlayerFactory.Singleton;

        protected override ICardFactory<PlayingCardSuit, PlayingCardRank> CardFactory =>
            (ICardFactory<PlayingCardSuit, PlayingCardRank>)PlayingCardFactory.Singleton;

        protected override int MinPlayers => 2;

        protected override int MaxPlayers => 2;

        protected override int StartingHandCardCount => 5;
    }
}
