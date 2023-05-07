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
        Ace,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
    }

    public enum ActiveEffect
    {
        Invalid,
        PlayCard,
    }
}
