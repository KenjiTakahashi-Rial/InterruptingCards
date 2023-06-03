using System.Collections.Generic;

using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public class TheStackManager : MonoBehaviour
    {
        private enum StackItemType
        {
            Invalid,
            Loot,
            Ability,
            DiceRoll,
        }

        private readonly CardConfig _cardConfig = CardConfig.Singleton;
        private readonly Stack<IStackItem> _theStack = new();

        [SerializeField] private StateMachineManager _stateMachineManager;

        public bool IsEmpty => _theStack.Count == 0;

        // Methods

        public void PushLoot(int cardId)
        {
            Debug.Log($"Pushing {_cardConfig.GetCardString(cardId)} to The Stack");
            _theStack.Push(new LootStackItem(cardId));
        }

        public void PushAbility(CardAbility ability)
        {
            Debug.Log($"Pushing {ability} to The Stack");
            _theStack.Push(new AbilityStackItem(ability));
        }

        public void PushDiceRoll(int diceRoll)
        {
            Debug.Log($"Pushing dice roll {diceRoll} to The Stack");
            _theStack.Push(new DiceRollStackItem(diceRoll));
        }

        public void Pop()
        {
            if (_stateMachineManager.CurrentState == StateMachine.Popping)
            {
                Debug.Log("Popping The Stack");
                _theStack.Pop();
            }
            else
            {
                Debug.Log($"Cannot pop The Stack in state {_stateMachineManager.CurrentStateName}");
            }
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