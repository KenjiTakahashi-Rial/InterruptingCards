using InterruptingCards.Models;

namespace InterruptingCards.Models
{
    public class PlayingCardPlayer : AbstractPlayer<PlayingCardSuit, PlayingCardRank>
    {
        public PlayingCardPlayer(ulong id, string name, IHand<PlayingCardSuit, PlayingCardRank> hand = null) : base(id, name, hand) { }
    }
}