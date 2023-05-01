namespace InterruptingCards.Models
{
// The "Enum" suffix differentiates from the Suit and Enum properties of ICard
#pragma warning disable S2344 // Enumeration type names should not have "Flags" or "Enum" suffixes
    public enum SuitEnum
    {
        Invalid,
        Clubs,
        Diamonds,
        Hearts,
        Spades,
    }

    public enum RankEnum
#pragma warning restore S2344 // Enumeration type names should not have "Flags" or "Enum" suffixes
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
