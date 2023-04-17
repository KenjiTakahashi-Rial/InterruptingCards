public class PlayingCard : ICard
{
    public enum PlayingCardSuit
    {
        Clubs,
        Diamonds,
        Hearts,
        Spades,
    }

    public enum PlayingCardAceHighRank
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

    public PlayingCardSuit Suit { get; private set; };

    public PlayingCardAceHighRank Rank { get; private set; };
}