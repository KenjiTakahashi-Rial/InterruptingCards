namespace InterruptingCards.Models
{
    public class PlayingCard : AbstractCard<PlayingCardSuit, PlayingCardRank>
    {
        public PlayingCard(PlayingCardSuit suit, PlayingCardRank rank) : base(suit, rank) { }

        public override ICard<PlayingCardSuit, PlayingCardRank> Clone()
        {
            return new PlayingCard(Suit, Rank);
        }
    }
}