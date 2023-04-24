using InterruptingCards.Factories;
using InterruptingCards.Models;

namespace InterruptingCards.Managers.GameManagers
{
    public class PlayingCardGameManager : AbstractGameManager<PlayingCardSuit, PlayingCardRank>
    {
        public PlayingCardGameManager(
            IPlayerFactory<PlayingCardSuit,PlayingCardRank> playerFactory,
            ICardFactory<PlayingCardSuit, PlayingCardRank> cardFactory
        ) : base(playerFactory, cardFactory) { }

        protected override IGameManagerNetworkDependency<PlayingCardSuit, PlayingCardRank> NetworkDependency =>
            PlayingCardGameManagerNetworkDependency.Singleton;

        protected override int MinPlayers => 2;

        protected override int MaxPlayers => 2;

        protected override void TryDealHands()
        {
            // TODO: this
            throw new System.NotImplementedException();
        }

        public override void DealHands()
        {
            // TODO: this
            throw new System.NotImplementedException();
        }
    }
}
