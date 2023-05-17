using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using InterruptingCards.Utilities;

namespace InterruptingCards.Config
{
    public class StateMachineConfig
    {
        private static readonly Dictionary<StateMachine, string> Strings = new()
        {
            // States
            { StateMachine.WaitingForAllReadyState, "Base." +               "WaitingForAllReady" },
            { StateMachine.WaitingForClientsState,  "Base." +               "WaitingForClients"  },
            { StateMachine.InitializingGameState,   "Base." +               "InitializingGame"   },
            { StateMachine.EndGameState,            "Base.InGame." +        "EndGame"            },
            { StateMachine.StartTurnState,          "Base.InGame.PlayerTurns.StartTurn"          },
            { StateMachine.EndTurnState,            "Base.InGame.PlayerTurns.EndTurn"            },
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

        public string GetName(int id)
        {
            return ReverseLookup[id].ToString();
        }
    }
}
