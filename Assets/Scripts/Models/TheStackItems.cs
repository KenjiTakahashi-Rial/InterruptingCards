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

        Player PushedBy { get; }
    }

    public class LootTheStackItem : ITheStackItem
    {
        public LootTheStackItem(int cardId) { CardId = cardId; }

        public TheStackItemType Type => TheStackItemType.Loot;

        public Player PushedBy { get; }

        public int CardId { get; }
    }

    public class AbilityTheStackItem : ITheStackItem
    {
        public AbilityTheStackItem(CardAbility ability) { Ability = ability; }

        public TheStackItemType Type => TheStackItemType.Ability;

        public Player PushedBy { get; }

        public CardAbility Ability { get; }
    }

    public class DiceRollTheStackItem : ITheStackItem
    {
        public DiceRollTheStackItem(int diceRoll) { DiceRoll = diceRoll; }

        public TheStackItemType Type => TheStackItemType.DiceRoll;

        public Player PushedBy { get; }

        public int DiceRoll { get; }
    }
}
