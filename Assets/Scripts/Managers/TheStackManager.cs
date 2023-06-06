using System;
using System.Collections.Generic;

using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public class TheStackManager : MonoBehaviour
    {
        public event Action<ITheStackItem> OnResolve;

        private readonly CardConfig _cardConfig = CardConfig.Singleton;
        private readonly Stack<ITheStackItem> _theStack = new();

        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private StateMachineManager _stateMachineManager;
        [SerializeField] private StateMachineManager _gameStateMachineManager;

        private Player _lastPushBy;

        public static TheStackManager Singleton { get; private set; }

        public Player PriorityPlayer { get; private set; }

        // Unity Methods

        public void Awake()
        {
            Singleton = this;
            _playerManager.OnActivePlayerChanged += SetActivePlayerPriority;
        }

        public void OnDestroy()
        {
            Singleton = null;
            _playerManager.OnActivePlayerChanged -= SetActivePlayerPriority;
        }

        // The Stack Operations

        public void PushLoot(int cardId, Player player)
        {
            Debug.Log($"Player {player.Name} pushing {_cardConfig.GetCardString(cardId)} to The Stack");
            _lastPushBy = player;
            _theStack.Push(new LootTheStackItem(cardId));
            _stateMachineManager.SetBool(StateMachine.TheStackIsEmpty, _theStack.Count == 0);
        }

        public void PushAbility(CardAbility ability, Player player)
        {
            Debug.Log($"Player {player.Name} pushing {ability} to The Stack");
            _lastPushBy = player;
            _theStack.Push(new AbilityTheStackItem(ability));
            _stateMachineManager.SetBool(StateMachine.TheStackIsEmpty, _theStack.Count == 0);
        }

        public void PushDiceRoll(int diceRoll, Player player)
        {
            Debug.Log($"Player {player.Name} pushing dice roll {diceRoll} to The Stack");
            _lastPushBy = player;
            _theStack.Push(new DiceRollTheStackItem(diceRoll));
            _stateMachineManager.SetBool(StateMachine.TheStackIsEmpty, _theStack.Count == 0);
        }

        public void Pop()
        {
            if (_stateMachineManager.CurrentState == StateMachine.TheStackPopping)
            {
                Debug.Log("Popping The Stack");
                var item = _theStack.Pop();
                PriorityPlayer = item.PushedBy;
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

        public void Begin()
        {
            _stateMachineManager.SetTrigger(StateMachine.TheStackBegin);
        }

        public void End()
        {
            if (_stateMachineManager.CurrentState == StateMachine.TheStackEnding)
            {
                _lastPushBy = null;
                _gameStateMachineManager.SetTrigger(StateMachine.GamePriorityPassComplete);
                _stateMachineManager.SetTrigger(StateMachine.TheStackEnded);
            }
            else
            {
                Debug.LogWarning($"Cannot end The Stack in state {_stateMachineManager.CurrentStateName}");
            }
        }

        // Priority Operations

        public void SetActivePlayerPriority(Player _)
        {
            PriorityPlayer = _playerManager.ActivePlayer;
        }

        public void PriorityPasses()
        {
            if (_stateMachineManager.CurrentState == StateMachine.TheStackPriorityPassing)
            {
                Debug.Log($"Priority passes");
                // TODO: Automatically passing priority for now. Change later
                PassPriority();
            }
            else
            {
                Debug.LogWarning($"Cannot pass priority in state {_stateMachineManager.CurrentStateName}");
            }
        }

        private void PassPriority()
        {
            var prevPriorityPlayer = PriorityPlayer;
            PriorityPlayer = _playerManager.GetNext(PriorityPlayer.Id);
            var nextPriorityPlayer = PriorityPlayer;
            Debug.Log($"Passing priority from {prevPriorityPlayer.Name} to {nextPriorityPlayer.Name}");

            if (PriorityPlayer == _lastPushBy || _lastPushBy == null && PriorityPlayer == _playerManager.ActivePlayer)
            {
                 _stateMachineManager.SetTrigger(StateMachine.TheStackPriorityPassComplete);
            }
            // TODO: Automatically passing priority for now. Change later
            else
            {
                PassPriority();
            }
        }
    }
}