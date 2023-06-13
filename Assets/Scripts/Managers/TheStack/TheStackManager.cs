using System;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Models;

namespace InterruptingCards.Managers.TheStack
{
    public class TheStackManager : NetworkBehaviour
    {
        private readonly CardConfig _cardConfig = CardConfig.Singleton;

        [Header("Managers")]
        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private StateMachineManager _stateMachineManager;
        [SerializeField] private StateMachineManager _gameStateMachineManager;

        [Header("Resolution")]
        [SerializeField] private ResolveLoot _resolveLoot;

        private NetworkList<TheStackElement> _theStack;

        public event Action<TheStackElement> OnResolve;

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
            Log.Info($"Player {player.Name} pushing {_cardConfig.GetName(cardId)} to The Stack");
            _theStack.Add(new TheStackElement(TheStackElementType.Loot, player.Id, cardId));
            Push(player);
        }

        public void PushAbility(Player player, CardAbility ability)
        {
            Log.Info($"Player {player.Name} pushing {ability} to The Stack");
            _theStack.Add(new TheStackElement(TheStackElementType.Ability, player.Id, (int)ability));
            Push(player);
        }

        public void PushDiceRoll(Player player, int diceRoll)
        {
            Log.Info($"Player {player.Name} pushing dice roll {diceRoll} to The Stack");
            _theStack.Add(new TheStackElement(TheStackElementType.DiceRoll, player.Id, diceRoll));
            Push(player);
        }

        public void Pop()
        {
            if (_stateMachineManager.CurrentState == StateMachine.TheStackPopping)
            {
                Log.Info("Popping The Stack");
                var last = _theStack.Count - 1;
                var element = _theStack[last];
                _theStack.RemoveAt(last);
                Resolve(element);
                _stateMachineManager.SetBool(StateMachine.TheStackIsEmpty, _theStack.Count == 0);
                _stateMachineManager.SetTrigger(StateMachine.TheStackPopped);
            }
            else
            {
                Log.Warn($"Cannot pop The Stack in state {_stateMachineManager.CurrentStateName}");
            }
        }

        // State Machine Operations

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

        // Helper Methods

        private void Push(Player player)
        {
            LastPushBy = player;
            _stateMachineManager.SetBool(StateMachine.TheStackIsEmpty, _theStack.Count == 0);
        }

        private void Resolve(TheStackElement element)
        {
            switch (element.Type)
            {
                case TheStackElementType.Loot:
                    _resolveLoot.Resolve(element);
                    break;
                case TheStackElementType.Ability:
                    // TODO
                    break;
                case TheStackElementType.DiceRoll:
                    // TODO
                    break;
                default:
                    throw new NotImplementedException();
            }

            OnResolve?.Invoke(element);
        }
    }
}