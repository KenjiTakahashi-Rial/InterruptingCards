using System;
using System.Collections.Generic;

using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public class TheStackManager : MonoBehaviour
    {
        public event Action<ITheStackElement> OnResolve;

        private readonly CardConfig _cardConfig = CardConfig.Singleton;
        private readonly Stack<ITheStackElement> _theStack = new();

        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private PriorityManager _priorityManager;
        [SerializeField] private StateMachineManager _stateMachineManager;
        [SerializeField] private StateMachineManager _gameStateMachineManager;

        public static TheStackManager Singleton { get; private set; }

        public Player LastPushBy { get; private set; }

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
            LastPushBy = player;
            _theStack.Push(new LootElement(cardId));
            _stateMachineManager.SetBool(StateMachine.TheStackIsEmpty, _theStack.Count == 0);
        }

        public void PushAbility(CardAbility ability, Player player)
        {
            Debug.Log($"Player {player.Name} pushing {ability} to The Stack");
            LastPushBy = player;
            _theStack.Push(new AbilityElement(ability));
            _stateMachineManager.SetBool(StateMachine.TheStackIsEmpty, _theStack.Count == 0);
        }

        public void PushDiceRoll(int diceRoll, Player player)
        {
            Debug.Log($"Player {player.Name} pushing dice roll {diceRoll} to The Stack");
            LastPushBy = player;
            _theStack.Push(new DiceRollElement(diceRoll));
            _stateMachineManager.SetBool(StateMachine.TheStackIsEmpty, _theStack.Count == 0);
        }

        public void Pop()
        {
            if (_stateMachineManager.CurrentState == StateMachine.TheStackPopping)
            {
                Debug.Log("Popping The Stack");
                var item = _theStack.Pop();
                OnResolve?.Invoke(item);
                _stateMachineManager.SetBool(StateMachine.TheStackIsEmpty, _theStack.Count == 0);
                _stateMachineManager.SetTrigger(StateMachine.TheStackPopped);
            }
            else
            {
                Debug.LogWarning($"Cannot pop The Stack in state {_stateMachineManager.CurrentStateName}");
            }
        }

        // State Machine Operations

        // TODO: Start putting actual elements on the stack and then remove this method
        public void Begin()
        {
            _stateMachineManager.SetTrigger(StateMachine.TheStackBegin);
        }

        public void End()
        {
            if (_stateMachineManager.CurrentState == StateMachine.TheStackEnding)
            {
                LastPushBy = null;
                _gameStateMachineManager.SetTrigger(StateMachine.GamePriorityPassComplete);
                _stateMachineManager.SetTrigger(StateMachine.TheStackEnded);
            }
            else
            {
                Debug.LogWarning($"Cannot end The Stack in state {_stateMachineManager.CurrentStateName}");
            }
        }
    }
}