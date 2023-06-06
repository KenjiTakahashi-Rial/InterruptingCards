namespace InterruptingCards.Models
{
    public enum TheStackElement
    {
        Invalid,
        Loot,
        Ability,
        DiceRoll,
    }

    public interface ITheStackElement
    {
        TheStackElement Type { get; }

        Player PushedBy { get; }
    }

    public class LootElement : ITheStackElement
    {
        public LootElement(int cardId) { CardId = cardId; }

        public TheStackElement Type => TheStackElement.Loot;

        public Player PushedBy { get; }

        public int CardId { get; }
    }

    public class AbilityElement : ITheStackElement
    {
        public AbilityElement(CardAbility ability) { Ability = ability; }

        public TheStackElement Type => TheStackElement.Ability;

        public Player PushedBy { get; }

        public CardAbility Ability { get; }
    }

    public class DiceRollElement : ITheStackElement
    {
        public DiceRollElement(int diceRoll) { DiceRoll = diceRoll; }

        public TheStackElement Type => TheStackElement.DiceRoll;

        public Player PushedBy { get; }

        public int DiceRoll { get; }
    }
}
