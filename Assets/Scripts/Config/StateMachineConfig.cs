using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using InterruptingCards.Utilities;

namespace InterruptingCards.Config
{
    public enum StateMachine
    {
        Invalid,

        // States
        WaitingForClientsState,
        WaitingForAllReadyState,
        InitializingGameState,
        EndingGameState,
        StartingTurnState,
        WaitingForDrawCardState,
        WaitingForPlayCardState,
        EndingTurnState,

        // Triggers
        AllReadyTrigger,
        DrawCardTrigger,
        ForceEndTurnTrigger,
        ForceEndGameTrigger,
        PlayCardTrigger,
        PlayCardActiveEffectTrigger,
        StartGameTrigger,
        WaitForReadyTrigger,
    }

    public class StateMachineConfig
    {
        private static readonly Dictionary<StateMachine, string> Strings = new()
        {
            // States
            { StateMachine.WaitingForAllReadyState, "Base." +               "WaitingForAllReady" },
            { StateMachine.WaitingForClientsState,  "Base." +               "WaitingForClients"  },
            { StateMachine.InitializingGameState,   "Base." +               "InitializingGame"   },
            { StateMachine.EndingGameState,         "Base.InGame." +        "EndingGame"         },
            { StateMachine.StartingTurnState,       "Base.InGame.PlayerTurns.StartingTurn"       },
            { StateMachine.EndingTurnState,         "Base.InGame.PlayerTurns.EndingTurn"         },
            { StateMachine.WaitingForDrawCardState, "Base.InGame.PlayerTurns.WaitingForDrawCard" },
            { StateMachine.WaitingForPlayCardState, "Base.InGame.PlayerTurns.WaitingForPlayCard" },

            // Triggers
            { StateMachine.AllReadyTrigger,             "allReady"             },
            { StateMachine.DrawCardTrigger,             "drawCard"             },
            { StateMachine.ForceEndTurnTrigger,         "forceEndTurn"         },
            { StateMachine.ForceEndGameTrigger,         "forceEndGame"         },
            { StateMachine.PlayCardTrigger,             "playCard"             },
            { StateMachine.PlayCardActiveEffectTrigger, "playCardActiveEffect" },
            { StateMachine.StartGameTrigger,            "startGame"            },
            { StateMachine.WaitForReadyTrigger,         "waitForReady"         },
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
