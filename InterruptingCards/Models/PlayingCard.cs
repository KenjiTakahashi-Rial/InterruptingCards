namespace InterruptingCards.Models
{
    public enum PlayingCardSuit
    {
        Clubs,
        Diamonds,
        Hearts,
        Spades,
    }

    public enum PlayingCardRankAceHigh
    {
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
        Ace = 14,
    }

    public class PlayingCard : ICard<PlayingCardSuit, PlayingCardRankAceHigh>
    {
        public PlayingCard(PlayingCardSuit suit, PlayingCardRankAceHigh rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public PlayingCardSuit Suit { get; }

        public PlayingCardRankAceHigh Rank { get; }
    }
}