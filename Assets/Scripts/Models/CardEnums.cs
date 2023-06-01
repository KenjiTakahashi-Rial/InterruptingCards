namespace InterruptingCards.Models

{
    public enum CardPack
    {
        Invalid,
        PlayingCards,
        InterruptingCards,
    }

    public enum CardSuit
    {
        Invalid,

        // Playing cards
        Clubs,
        Diamonds,
        Hearts,
        Spades,

        // Interrupting cards
        InterruptingSuit,
    }

    public enum CardRank
    {
        Invalid,

        // Playing cards
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

        // Interrupting cards
        InterruptingRank,
    }

    public enum CardActiveEffect
    {
        Invalid,
        PlayCard,
    }
}
