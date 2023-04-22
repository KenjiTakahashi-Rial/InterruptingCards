using InterruptingCards.Factories;
using InterruptingCards.Models;
using InterruptingCards.Serialization;

namespace InterruptingCards.Managers.GameManagers
{
    public class PlayingCardGameManager : AbstractGameManager<PlayingCardSuit, PlayingCardRank, SerializedPlayingCard>
    {
        public PlayingCardGameManager(IPlayerFactory<PlayingCardSuit, PlayingCardRank> playerFactory) : base(playerFactory) { }

        protected internal override IGameManagerNetworkDependency NetworkDependency => PlayingCardGameManagerNetworkDependency.Singleton;

        protected internal override int MinPlayers => 2;

        protected internal override int MaxPlayers => 2;

        protected override void DealHands()
        {
            throw new System.NotImplementedException();
        }

        internal override void DealHandsServerRpc()
        {
            throw new System.NotImplementedException();
        }
    }
}
