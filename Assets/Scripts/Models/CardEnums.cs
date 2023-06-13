namespace InterruptingCards.Models

{
    public enum CardPack
    {
        Invalid,
        PlayingCards,
        FourSouls,
    }

    public enum CardSuit
    {
        Invalid,

        // Playing cards
        Clubs,
        Diamonds,
        Hearts,
        Spades,

        // Four Souls
        Characters,
        StartingItems,
        Treasure,
        Loot,
        Monsters,
        BonusSouls,
        Rooms,
        OutsideCards,
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

        // Four Souls
        APenny,
        TwoCents,
        ThreeCents,
        FourCents,
        ANickel,
        ADime,
    }

    public enum CardPlayedEffect
    {
        Invalid,
        GainCents,
    }

    public enum CardActiveEffect
    {
        Invalid,
    }

    public enum CardAbility
    {
        Invalid,
    }
}
