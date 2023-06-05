using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using InterruptingCards.Utilities;

namespace InterruptingCards.Config
{
    public enum StateMachine
    {
        Invalid,

        // Game States
        WaitingForClients,
        WaitingForAllReady,
        InitializingGame,

        // In-Game
        Entry,
        EndingGame,

        // Start Phase
        Recharging,
        StartPhaseTriggeringAbilities,
        Looting,
        GamePriorityPassing,

        // Action Phase
        Idling,
        DeclaringAttack,
        Attacking,
        DeclaringPurchase,
        Purchasing,
        PerformingAction,
        DeclaringEndTurn,

        // EndPhase
        EndPhaseTriggeringAbilities,
        Discarding,
        ShiftingRoom,
        TurnEnding,

        // Game Triggers
        ActionPhaseComplete,
        AllReady,
        AttackComplete,
        DeclareAttack,
        DeclareEndTurn,
        DeclarePurchase,
        DiscardComplete,
        EndGame,
        EndTurn,
        EndPhaseComplete,
        ForceEndGame,
        ForceEndTurn,
        GamePriorityPassComplete,
        LootComplete,
        PerformAction,
        PurchaseComplete,
        RechargeComplete,
        ShiftRoomComplete,
        StartGame,
        StartPhaseComplete,
        WaitForReady,

        // The Stack States
        TheStackPriorityPassing,
        Popping,

        // The Stack Triggers
        TheStackPriorityPassComplete,
        TheStackIsEmpty,
    }

    public class StateMachineConfig
    {
        private static readonly Dictionary<StateMachine, string> Strings = new()
        {
            // Game States
            { StateMachine.WaitingForClients, "Base.WaitingForClients" },
            { StateMachine.WaitingForAllReady, "Base.WaitingForAllReady" },

            // In-Game
            { StateMachine.Entry, "Base.InGame.Entry" },
            { StateMachine.EndingGame, "Base.InGame.EndingGame" },

            // Start Phase
            { StateMachine.Recharging, "Base.InGame.PlayerTurns.StartPhase.Recharging"},
            { StateMachine.StartPhaseTriggeringAbilities, "Base.InGame.PlayerTurns.StartPhase.TriggeringAbilities"},
            { StateMachine.Looting, "Base.InGame.PlayerTurns.StartPhase.Looting"},
            { StateMachine.GamePriorityPassing, "Base.InGame.PlayerTurns.StartPhase.PriorityPassing"},

            // Action Phase
            { StateMachine.Idling, "Base.InGame.PlayerTurns.ActionPhase.Idling"},
            { StateMachine.DeclaringAttack, "Base.InGame.PlayerTurns.ActionPhase.DeclaringAttack"},
            { StateMachine.Attacking, "Base.InGame.PlayerTurns.ActionPhase.Attacking"},
            { StateMachine.DeclaringPurchase, "Base.InGame.PlayerTurns.ActionPhase.DeclaringPurchase"},
            { StateMachine.Purchasing, "Base.InGame.PlayerTurns.ActionPhase.Purchasing"},
            { StateMachine.PerformingAction, "Base.InGame.PlayerTurns.ActionPhase.PerformingAction"},
            { StateMachine.DeclaringEndTurn, "Base.InGame.PlayerTurns.ActionPhase.DeclaringEndTurn"},

            // End Phase
            { StateMachine.EndPhaseTriggeringAbilities, "Base.InGame.PlayerTurns.EndPhase.TriggeringAbilities"},
            { StateMachine.Discarding, "Base.InGame.PlayerTurns.EndPhase.Discarding"},
            { StateMachine.ShiftingRoom, "Base.InGame.PlayerTurns.EndPhase.ShiftingRoom"},
            { StateMachine.TurnEnding, "Base.InGame.PlayerTurns.EndPhase.TurnEnding"},

            // Game Triggers
            { StateMachine.ActionPhaseComplete, "actionPhaseComplete"},
            { StateMachine.AllReady, "allReady"},
            { StateMachine.AttackComplete, "attackComplete"},
            { StateMachine.DeclareAttack, "declareAttack"},
            { StateMachine.DeclareEndTurn, "declareEndTurn"},
            { StateMachine.DeclarePurchase, "declarePurchase"},
            { StateMachine.DiscardComplete, "discardComplete"},
            { StateMachine.EndGame, "endGame"},
            { StateMachine.EndTurn, "endTurn"},
            { StateMachine.EndPhaseComplete, "endPhaseComplete"},
            { StateMachine.ForceEndGame, "forceEndGame"},
            { StateMachine.ForceEndTurn, "forceEndTurn"},
            { StateMachine.GamePriorityPassComplete, "gamePriorityPassComplete"},
            { StateMachine.LootComplete, "lootComplete"},
            { StateMachine.PerformAction, "performAction"},
            { StateMachine.PurchaseComplete, "purchaseComplete"},
            { StateMachine.RechargeComplete, "rechargeComplete"},
            { StateMachine.ShiftRoomComplete, "shiftRoomComplete"},
            { StateMachine.StartGame, "startGame"},
            { StateMachine.StartPhaseComplete, "startPhaseComplete"},

            // The Stack States
            { StateMachine.TheStackPriorityPassing, "Base.PriorityPassing" },
            { StateMachine.Popping, "Base.Popping" },

            // The Stack Triggers
            { StateMachine.TheStackPriorityPassComplete, "theStackPriorityPassComplete" },
            { StateMachine.TheStackIsEmpty, "Base.IsEmpty" },
        };

        private static readonly ImmutableDictionary<StateMachine, int> Ids = 
            new(Strings.ToDictionary(pair => pair.Key, pair => Animator.StringToHash(pair.Value)));

        private static readonly ImmutableDictionary<int, StateMachine> ReverseLookup =
            Ids.ToDictionary(pair => pair.Value, pair => pair.Key);

        private StateMachineConfig() { }

        public static StateMachineConfig Singleton { get; } = new();

        public int GetId(StateMachine e)
        {
            return Ids[e];
        }

        public StateMachine GetEnum(int id)
        {
            return ReverseLookup[id];
        }

        public string GetName(int id)
        {
            return GetEnum(id).ToString();
        }
    }
}
