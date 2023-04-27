using Unity.Netcode;

namespace InterruptingCards.Models
{
    public class PlayingCard : AbstractCard
    {
        public PlayingCard() : base() { }

        public PlayingCard(SuitEnum suit, RankEnum rank) : base(suit, rank) { }

        public override object Clone()
        {
            return new PlayingCard(Suit, Rank);
        }
    }
}