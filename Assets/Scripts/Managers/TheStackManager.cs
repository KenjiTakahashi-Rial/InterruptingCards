using System;
using System.Collections.Generic;

using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public class TheStackManager : MonoBehaviour
    {
        public event Action<ITheStackItem> OnPop;

        private readonly CardConfig _cardConfig = CardConfig.Singleton;
        private readonly Stack<ITheStackItem> _theStack = new();

        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private StateMachineManager _stateMachineManager;

        private Player _lastItemPlayer;

        public bool IsEmpty => _theStack.Count == 0;

        public Player PriorityPlayer { get; private set; }

        // Methods

        public void PushLoot(int cardId, Player player)
        {
            Debug.Log($"Player {player.Name} pushing {_cardConfig.GetCardString(cardId)} to The Stack");
            _theStack.Push(new LootTheStackItem(cardId));
            _lastItemPlayer = player;
        }

        public void PushAbility(CardAbility ability, Player player)
        {
            Debug.Log($"Player {player.Name} pushing {ability} to The Stack");
            _theStack.Push(new AbilityTheStackItem(ability));
            _lastItemPlayer = player;
        }

        public void PushDiceRoll(int diceRoll, Player player)
        {
            Debug.Log($"Player {player.Name} pushing dice roll {diceRoll} to The Stack");
            _theStack.Push(new DiceRollTheStackItem(diceRoll));
            _lastItemPlayer = player;
        }

        public void PriorityPasses(Player startingPlayer)
        {
            Debug.Log($"Priority passes starting with {startingPlayer.Name}");
            _lastItemPlayer = startingPlayer;
        }

        public void PassPriority()
        {
            var prevPriorityPlayer = PriorityPlayer;
            PriorityPlayer = _playerManager.GetNext(PriorityPlayer.Id);
            var nextPriorityPlayer = PriorityPlayer;
            Debug.Log($"Passing priority from {prevPriorityPlayer} to {nextPriorityPlayer}");

            if (PriorityPlayer == _lastItemPlayer)
            {
                Pop();
            }
        }

        private void Pop()
        {
            if (_stateMachineManager.CurrentState == StateMachine.Popping)
            {
                Debug.Log("Popping The Stack");
                OnPop?.Invoke(_theStack.Pop());
            }
            else
            {
                Debug.Log($"Cannot pop The Stack in state {_stateMachineManager.CurrentStateName}");
            }
        }
    }
}