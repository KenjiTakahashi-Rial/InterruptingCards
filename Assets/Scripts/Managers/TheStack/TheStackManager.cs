using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Behaviours;
using InterruptingCards.Config;
using InterruptingCards.Models;

namespace InterruptingCards.Managers.TheStack
{
    public class TheStackManager : NetworkBehaviour
    {
        private readonly CardConfig _cardConfig = CardConfig.Singleton;

        [SerializeField] private ResolveLoot _resolveLoot;
        [SerializeField] private ResolveAbility _resolveAbility;

        private NetworkList<TheStackElement> _theStack;

        public event Action OnEnd;
        public event Action<TheStackElement> OnResolve;

        public bool IsEmpty => _theStack.Count == 0;
        public PlayerBehaviour LastPushBy { get; private set; }

        private GameManager Game => GameManager.Singleton;
        private LogManager Log => LogManager.Singleton;
        private StateMachineManager StateMachineManager => Game.TheStackStateMachineManager;

        // Unity Methods

        public void Awake()
        {
            _theStack = new();
        }

        // TODO: On client disconnect mid-game, clear The Stack and set state to Idling

        public override void OnDestroy()
        {
            _theStack = null;
            base.OnDestroy();
        }

        // The Stack Operations

        public void PushLoot(PlayerBehaviour player, int cardId)
        {
            Log.Info($"Player {player.Name} pushing {_cardConfig.GetName(cardId)} to The Stack");
            var element = new TheStackElement(TheStackElementType.Loot, player.Id, cardId);
            Push(player, element);
        }

        public void PushAbility(PlayerBehaviour player, CardAbility ability)
        {
            Log.Info($"Player {player.Name} pushing {ability} to The Stack");
            var element = new TheStackElement(TheStackElementType.Ability, player.Id, (int)ability);
            Push(player, element);
        }

        public void PushDiceRoll(PlayerBehaviour player, int diceRoll)
        {
            Log.Info($"Player {player.Name} pushing dice roll {diceRoll} to The Stack");
            var element = new TheStackElement(TheStackElementType.DiceRoll, player.Id, diceRoll);
            Push(player, element);
        }

        public void Pop()
        {
            if (!IsServer)
            {
                return;
            }

            if (StateMachineManager.CurrentState == StateMachine.TheStackPopping)
            {
                Log.Info("Popping The Stack");
                var last = _theStack.Count - 1;
                var element = _theStack[last];
                _theStack.RemoveAt(last);
                Resolve(element);
                StateMachineManager.SetTrigger(StateMachine.TheStackPopped);
            }
            else
            {
                Log.Warn($"Cannot pop The Stack from {StateMachineManager.CurrentStateName}");
            }
        }

        // State Machine Operations

        public void Begin()
        {
            StateMachineManager.SetTrigger(StateMachine.TheStackBegin);
        }

        public void End()
        {
            if (!IsServer)
            {
                return;
            }

            if (StateMachineManager.CurrentState == StateMachine.TheStackEnding)
            {
                LastPushBy = null;
                StateMachineManager.SetTrigger(StateMachine.TheStackEnded);
                OnEnd?.Invoke();
            }
            else
            {
                Log.Warn($"Cannot end The Stack from {StateMachineManager.CurrentStateName}");
            }
        }

        // Helper Methods

        public TheStackElement[] DebugTopN(int n)
        {
            var length = _theStack.Count < n ? _theStack.Count : n;
            var topN = new TheStackElement[length];
            for (var i = 0; i < length; i++)
            {
                topN[i] = _theStack[_theStack.Count - i - 1];
            }
            return topN;
        }

        private void Push(PlayerBehaviour player, TheStackElement element)
        {
            LastPushBy = player;
            
            if (IsEmpty && StateMachineManager.CurrentState == StateMachine.TheStackIdling)
            {
                StateMachineManager.SetTrigger(StateMachine.TheStackBegin);
            }

            _theStack.Add(element);
        }

        private void Resolve(TheStackElement element)
        {
            switch (element.Type)
            {
                case TheStackElementType.Loot:
                    _resolveLoot.Resolve(element);
                    break;
                case TheStackElementType.Ability:
                    _resolveAbility.Resolve(element);
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