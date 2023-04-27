using Unity.Netcode;

namespace InterruptingCards.Models
{
    public enum PlayingCardSuit
    {
        Invalid,
        Clubs,
        Diamonds,
        Hearts,
        Spades,
    }

    public enum PlayingCardRank
    {
        Invalid,
        Ace = 1,
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
    }
}
