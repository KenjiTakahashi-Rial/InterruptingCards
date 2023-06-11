using System;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public class TheStackManager : NetworkBehaviour
    {
        public event Action<TheStackElement> OnResolve;

        private readonly CardConfig _cardConfig = CardConfig.Singleton;

        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private StateMachineManager _stateMachineManager;
        [SerializeField] private StateMachineManager _gameStateMachineManager;

        private NetworkList<TheStackElement> _theStack;

        public static TheStackManager Singleton { get; private set; }

        public Player LastPushBy { get; private set; }

        private LogManager Log => LogManager.Singleton;

        // Unity Methods

        public void Awake()
        {
            Singleton = this;
            _theStack = new();
        }

        // TODO: On client disconnect mid-game, clear The Stack and set state to Idling

        public override void OnDestroy()
        {
            Singleton = null;
            _theStack = null;
            base.OnDestroy();
        }

        // The Stack Operations

        public void PushLoot(Player player, int cardId)
        {
            Log.Info($"Player {player.Name} pushing {_cardConfig.GetCardString(cardId)} to The Stack");
            LastPushBy = player;
            _theStack.Add(new TheStackElement(TheStackElementType.Loot, player.Id, cardId));
            _stateMachineManager.SetBool(StateMachine.TheStackIsEmpty, _theStack.Count == 0);
        }

        public void PushAbility(Player player, CardAbility ability)
        {
            Log.Info($"Player {player.Name} pushing {ability} to The Stack");
            LastPushBy = player;
            _theStack.Add(new TheStackElement(TheStackElementType.Ability, player.Id, (int)ability));
            _stateMachineManager.SetBool(StateMachine.TheStackIsEmpty, _theStack.Count == 0);
        }

        public void PushDiceRoll(Player player, int diceRoll)
        {
            Log.Info($"Player {player.Name} pushing dice roll {diceRoll} to The Stack");
            LastPushBy = player;
            _theStack.Add(new TheStackElement(TheStackElementType.DiceRoll, player.Id, diceRoll));
            _stateMachineManager.SetBool(StateMachine.TheStackIsEmpty, _theStack.Count == 0);
        }

        public void Pop()
        {
            if (_stateMachineManager.CurrentState == StateMachine.TheStackPopping)
            {
                Log.Info("Popping The Stack");
                var last = _theStack.Count - 1;
                var item = _theStack[last];
                _theStack.RemoveAt(last);
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
            if (!IsServer)
            {
                return;
            }

            if (_stateMachineManager.CurrentState == StateMachine.TheStackEnding)
            {
                LastPushBy = null;
                _gameStateMachineManager.SetTrigger(StateMachine.GamePriorityPassComplete);
                _stateMachineManager.SetTrigger(StateMachine.TheStackEnded);
            }
            else
            {
                Log.Warn($"Cannot end The Stack from {_stateMachineManager.CurrentStateName}");
            }
        }
    }
}