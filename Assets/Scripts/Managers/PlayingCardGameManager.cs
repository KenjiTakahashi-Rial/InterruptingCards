using InterruptingCards.Factories;
using InterruptingCards.Models;

namespace InterruptingCards.Managers.GameManagers
{
    public class PlayingCardGameManager : AbstractGameManager<PlayingCardSuit, PlayingCardRank>
    {
        public PlayingCardGameManager(IPlayerFactory<PlayingCardSuit, PlayingCardRank> playerFactory) : base(playerFactory) { }

        protected override void DealHands()
        {
            throw new System.NotImplementedException();
        }
    }
}
