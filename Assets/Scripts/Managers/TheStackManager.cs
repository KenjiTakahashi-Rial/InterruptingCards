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
        [SerializeField] private StateMachineManager _gameStateMachineManager;

        private bool _isPriorityPassFromStack;

        public static TheStackManager Singleton { get; private set; }

        public bool IsEmpty => _theStack.Count == 0;

        public Player PriorityPlayer { get; private set; }

        // Unity Methods

        public void Awake()
        {
            Singleton = this;
        }

        public void OnDestroy()
        {
            Singleton = null;
        }

        // The Stack Operations

        public void PushLoot(int cardId, Player player)
        {
            Debug.Log($"Player {player.Name} pushing {_cardConfig.GetCardString(cardId)} to The Stack");
            _theStack.Push(new LootTheStackItem(cardId));
        }

        public void PushAbility(CardAbility ability, Player player)
        {
            Debug.Log($"Player {player.Name} pushing {ability} to The Stack");
            _theStack.Push(new AbilityTheStackItem(ability));
        }

        public void PushDiceRoll(int diceRoll, Player player)
        {
            Debug.Log($"Player {player.Name} pushing dice roll {diceRoll} to The Stack");
            _theStack.Push(new DiceRollTheStackItem(diceRoll));
        }

        public void Pop()
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

        // Priority Operations

        public void PriorityPasses(bool isFromStack = false)
        {
            Debug.Log($"Priority passes (from stack: {isFromStack})");
            _isPriorityPassFromStack = isFromStack;
            // TODO: Automatically passing priority for now. Change later
            PassPriority();
        }

        public void PassPriority()
        {
            var prevPriorityPlayer = PriorityPlayer;
            PriorityPlayer = _playerManager.GetNext(PriorityPlayer.Id);
            var nextPriorityPlayer = PriorityPlayer;
            Debug.Log($"Passing priority from {prevPriorityPlayer} to {nextPriorityPlayer}");

            if (PriorityPlayer == _playerManager.ActivePlayer)
            {
                Pop();

                if (IsEmpty)
                {
                    if (_isPriorityPassFromStack)
                    {
                        _stateMachineManager.SetTrigger(StateMachine.TheStackPriorityPassComplete);
                    }
                    else
                    {
                        _gameStateMachineManager.SetTrigger(StateMachine.GamePriorityPassComplete);
                    }
                }
            }
            // TODO: Automatically passing priority for now. Change later
            else
            {
                PassPriority();
            }
        }
    }
}