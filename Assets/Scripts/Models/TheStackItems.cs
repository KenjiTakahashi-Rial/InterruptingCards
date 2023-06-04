using System;

namespace InterruptingCards.Models
{
    public enum TheStackItemType
    {
        Invalid,
        Loot,
        Ability,
        DiceRoll,
    }

    public interface ITheStackItem
    {
        TheStackItemType Type { get; }
    }

    public class LootTheStackItem : ITheStackItem
    {
        public LootTheStackItem(int cardId) { CardId = cardId; }

        public TheStackItemType Type => TheStackItemType.Loot;

        public int CardId { get; }
    }

    public class AbilityTheStackItem : ITheStackItem
    {
        public AbilityTheStackItem(CardAbility ability) { Ability = ability; }

        public TheStackItemType Type => TheStackItemType.Ability;

        public CardAbility Ability { get; }
    }

    public class DiceRollTheStackItem : ITheStackItem
    {
        public DiceRollTheStackItem(int diceRoll) { DiceRoll = diceRoll; }

        public TheStackItemType Type => TheStackItemType.DiceRoll;

        public int DiceRoll { get; }
    }
}
