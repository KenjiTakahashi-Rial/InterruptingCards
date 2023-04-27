using Unity.Netcode;

namespace InterruptingCards.Models
{
    public class PlayingCard : AbstractCard
    {
        public PlayingCard(SuitEnum suit = SuitEnum.Invalid, RankEnum rank = RankEnum.Invalid) : base(suit, rank) { }
        
        public override object Clone()
        {
            return new PlayingCard(Suit, Rank);
        }
    }
}