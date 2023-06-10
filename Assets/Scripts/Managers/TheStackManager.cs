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
        // TODO: Change to NetworkList
        private readonly Stack<ITheStackElement> _theStack = new();

        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private PriorityManager _priorityManager;
        [SerializeField] private StateMachineManager _stateMachineManager;
        [SerializeField] private StateMachineManager _gameStateMachineManager;

        public static TheStackManager Singleton { get; private set; }

        public Player LastPushBy { get; private set; }

        private LogManager Log => LogManager.Singleton;

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
            Log.Info($"Player {player.Name} pushing {_cardConfig.GetCardString(cardId)} to The Stack");
            LastPushBy = player;
            _theStack.Push(new LootElement(cardId));
            _stateMachineManager.SetBool(StateMachine.TheStackIsEmpty, _theStack.Count == 0);
        }

        public void PushAbility(CardAbility ability, Player player)
        {
            Log.Info($"Player {player.Name} pushing {ability} to The Stack");
            LastPushBy = player;
            _theStack.Push(new AbilityElement(ability));
            _stateMachineManager.SetBool(StateMachine.TheStackIsEmpty, _theStack.Count == 0);
        }

        public void PushDiceRoll(int diceRoll, Player player)
        {
            Log.Info($"Player {player.Name} pushing dice roll {diceRoll} to The Stack");
            LastPushBy = player;
            _theStack.Push(new DiceRollElement(diceRoll));
            _stateMachineManager.SetBool(StateMachine.TheStackIsEmpty, _theStack.Count == 0);
        }

        public void Pop()
        {
            if (_stateMachineManager.CurrentState == StateMachine.TheStackPopping)
            {
                Log.Info("Popping The Stack");
                var item = _theStack.Pop();
                OnResolve?.Invoke(item);
                _stateMachineManager.SetBool(StateMachine.TheStackIsEmpty, _theStack.Count == 0);
                _stateMachineManager.SetTrigger(StateMachine.TheStackPopped);
            }
            else
            {
                Log.Warn($"Cannot pop The Stack in state {_stateMachineManager.CurrentStateName}");
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
                Log.Warn($"Cannot end The Stack in state {_stateMachineManager.CurrentStateName}");
            }
        }
    }
}