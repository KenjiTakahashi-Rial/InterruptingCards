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

        // Four Souls Characters
        Isaac,
        Maggie,
        Cain,
        Judas,
        BlueBaby,
        Eve,
        Samson,
        Lazarus,
        Lilith,
        TheForgotten,
        Eden,
        Azazel,
        TheLost,
        TheKeeper,
        Apollyon,
        BumBo,

        // Four Souls Loot
        APenny,
        TwoCents,
        ThreeCents,
        FourCents,
        ANickel,
        ADime,
    }

    public enum CardAbility
    {
        Invalid,
        GainCents,
        AddLootPlay,
    }
}
