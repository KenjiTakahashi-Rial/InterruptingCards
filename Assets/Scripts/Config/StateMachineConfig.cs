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
        AddingLootPlayAndPurchase,
        ActionPhaseIdling,
        DeclaringAttack,
        Attacking,
        DeclaringPurchase,
        Purchasing,
        PlayingLoot,
        ActivatingAbility,
        DeclaringEndTurn,

        // EndPhase
        EndPhaseTriggeringAbilities,
        Discarding,
        ShiftingRoom,
        TurnEnding,

        // Game Triggers
        ActionPhaseComplete,
        ActivateAbility,
        ActivateAbilityComplete,
        AddLootPlayAndPurchaseComplete,
        AllReady,
        AttackComplete,
        DeclareAttack,
        DeclareAttackComplete,
        DeclareEndTurn,
        DeclareEndTurnComplete,
        DeclarePurchase,
        DeclarePurchaseComplete,
        DiscardComplete,
        EndGame,
        EndTurn,
        EndPhaseComplete,
        EndPhaseTriggerAbilitiesComplete,
        ForceEndGame,
        ForceEndTurn,
        GamePriorityPassComplete,
        LootComplete,
        PlayLoot,
        PlayLootComplete,
        PurchaseComplete,
        RechargeComplete,
        ShiftRoomComplete,
        StartGame,
        StartPhaseComplete,
        StartPhaseTriggerAbilitiesComplete,
        WaitForReady,

        // The Stack States
        TheStackIdling,
        TheStackPriorityPassing,
        TheStackPopping,
        TheStackEnding,

        // The Stack Triggers
        TheStackEnd,
        TheStackEnded,
        TheStackPop,
        TheStackPopped,
        TheStackBegin,
    }

    public sealed class StateMachineConfig
    {
        private static readonly Dictionary<StateMachine, string> Strings = new()
        {
            // Game States
            { StateMachine.WaitingForClients, "Base.WaitingForClients" },
            { StateMachine.WaitingForAllReady, "Base.WaitingForAllReady" },
            { StateMachine.InitializingGame, "Base.InitializingGame" },

            // In-Game
            { StateMachine.Entry, "Base.InGame.Entry" },
            { StateMachine.EndingGame, "Base.InGame.EndingGame" },

            // Start Phase
            { StateMachine.Recharging, "Base.InGame.PlayerTurns.StartPhase.Recharging"},
            { StateMachine.StartPhaseTriggeringAbilities, "Base.InGame.PlayerTurns.StartPhase.TriggeringAbilities"},
            { StateMachine.Looting, "Base.InGame.PlayerTurns.StartPhase.Looting"},
            { StateMachine.GamePriorityPassing, "Base.InGame.PlayerTurns.StartPhase.PriorityPassing"},

            // Action Phase
            { StateMachine.AddingLootPlayAndPurchase, "Base.InGame.PlayerTurns.ActionPhase.AddingLootPlayAndPurchase"},
            { StateMachine.ActionPhaseIdling, "Base.InGame.PlayerTurns.ActionPhase.Idling"},
            { StateMachine.DeclaringAttack, "Base.InGame.PlayerTurns.ActionPhase.DeclaringAttack"},
            { StateMachine.Attacking, "Base.InGame.PlayerTurns.ActionPhase.Attacking"},
            { StateMachine.DeclaringPurchase, "Base.InGame.PlayerTurns.ActionPhase.DeclaringPurchase"},
            { StateMachine.Purchasing, "Base.InGame.PlayerTurns.ActionPhase.Purchasing"},
            { StateMachine.PlayingLoot, "Base.InGame.PlayerTurns.ActionPhase.PlayingLoot"},
            { StateMachine.ActivatingAbility, "Base.InGame.PlayerTurns.ActionPhase.ActivatingAbility"},
            { StateMachine.DeclaringEndTurn, "Base.InGame.PlayerTurns.ActionPhase.DeclaringEndTurn"},

            // End Phase
            { StateMachine.EndPhaseTriggeringAbilities, "Base.InGame.PlayerTurns.EndPhase.TriggeringAbilities"},
            { StateMachine.Discarding, "Base.InGame.PlayerTurns.EndPhase.Discarding"},
            { StateMachine.ShiftingRoom, "Base.InGame.PlayerTurns.EndPhase.ShiftingRoom"},
            { StateMachine.TurnEnding, "Base.InGame.PlayerTurns.EndPhase.TurnEnding"},

            // Game Triggers
            { StateMachine.ActionPhaseComplete, "actionPhaseComplete"},
            { StateMachine.ActivateAbility, "activateAbility"},
            { StateMachine.ActivateAbilityComplete, "activateAbilityComplete"},
            { StateMachine.AddLootPlayAndPurchaseComplete, "addLootPlayAndPurchaseComplete"},
            { StateMachine.AllReady, "allReady"},
            { StateMachine.AttackComplete, "attackComplete"},
            { StateMachine.DeclareAttack, "declareAttack"},
            { StateMachine.DeclareAttackComplete, "declareAttackComplete"},
            { StateMachine.DeclareEndTurn, "declareEndTurn"},
            { StateMachine.DeclareEndTurnComplete, "declareEndTurnComplete"},
            { StateMachine.DeclarePurchase, "declarePurchase"},
            { StateMachine.DeclarePurchaseComplete, "declarePurchaseComplete"},
            { StateMachine.DiscardComplete, "discardComplete"},
            { StateMachine.EndGame, "endGame"},
            { StateMachine.EndTurn, "endTurn"},
            { StateMachine.EndPhaseComplete, "endPhaseComplete"},
            { StateMachine.EndPhaseTriggerAbilitiesComplete, "endPhaseTriggerAbilitiesComplete"},
            { StateMachine.ForceEndGame, "forceEndGame"},
            { StateMachine.ForceEndTurn, "forceEndTurn"},
            { StateMachine.GamePriorityPassComplete, "gamePriorityPassComplete"},
            { StateMachine.LootComplete, "lootComplete"},
            { StateMachine.PlayLoot, "playLoot"},
            { StateMachine.PlayLootComplete, "playLootComplete"},
            { StateMachine.PurchaseComplete, "purchaseComplete"},
            { StateMachine.RechargeComplete, "rechargeComplete"},
            { StateMachine.ShiftRoomComplete, "shiftRoomComplete"},
            { StateMachine.StartGame, "startGame"},
            { StateMachine.StartPhaseComplete, "startPhaseComplete"},
            { StateMachine.StartPhaseTriggerAbilitiesComplete, "startPhaseTriggerAbilitiesComplete"},
            { StateMachine.WaitForReady, "waitForReady" },

            // The Stack States
            { StateMachine.TheStackIdling, "Base.Idling" },
            { StateMachine.TheStackPriorityPassing, "Base.PriorityPassing" },
            { StateMachine.TheStackPopping, "Base.Popping" },
            { StateMachine.TheStackEnding, "Base.Ending" },

            // The Stack Triggers
            { StateMachine.TheStackBegin, "begin" },
            { StateMachine.TheStackEnd, "end" },
            { StateMachine.TheStackEnded, "ended" },
            { StateMachine.TheStackPop, "pop" },
            { StateMachine.TheStackPopped, "popped" },
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
