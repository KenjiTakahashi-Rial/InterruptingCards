using System.Collections.Generic;

using Unity.Netcode;

using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public class TheStackManager : NetworkBehaviour
    {
        private enum StackItemType
        {
            Invalid,
            Loot,
            Ability,
            DiceRoll,
        }

        private readonly Stack<IStackItem> _theStack = new();

        public bool IsEmpty => _theStack.Count == 0;

        // Methods

        public void PushLoot(int cardId)
        {
            _theStack.Push(new LootStackItem(cardId));
        }

        public void PushAbility(CardAbility ability)
        {
            _theStack.Push(new AbilityStackItem(ability));
        }

        public void PushDiceRoll(int diceRoll)
        {
            _theStack.Push(new DiceRollStackItem(diceRoll));
        }

        public void Pop()
        {
            _theStack.Pop();
        }

        // Models

        private interface IStackItem
        {
            StackItemType Type { get; }
        }

        private sealed class LootStackItem : IStackItem
        {
            public LootStackItem(int cardId) { CardId = cardId; }

            public StackItemType Type => StackItemType.Loot;

            public int CardId { get; }
        }

        private sealed class AbilityStackItem : IStackItem
        {
            public AbilityStackItem(CardAbility ability) { Ability = ability; }

            public StackItemType Type => StackItemType.Ability;

            public CardAbility Ability { get; }
        }

        private sealed class DiceRollStackItem : IStackItem
        {
            public DiceRollStackItem(int diceRoll) { DiceRoll = diceRoll; }

            public StackItemType Type => StackItemType.DiceRoll;

            public int DiceRoll { get; }
        }
    }
}