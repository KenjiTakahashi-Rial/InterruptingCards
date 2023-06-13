using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Managers;

namespace InterruptingCards.Actions
{
    public class PlayLootAction : AbstractCardAction
    {
        private readonly CardConfig _cardConfig = CardConfig.Singleton;

        private PriorityManager PriorityManager => Game.PriorityManager;
        private StateMachineManager TheStackStateMachineManager => Game.TheStackStateMachineManager;

        protected override bool CanExecute(ulong playerId, int cardId)
        {
            var priorityPlayer = PriorityManager.PriorityPlayer;

            if (playerId != priorityPlayer.Id)
            {
                Log.Warn(
                    $"Cannot play loot if not priority player (priority player: " +
                    $"{PriorityManager.PriorityPlayer.Name})");
                return false;
            }

            var lootPlays = priorityPlayer.LootPlays;
            if (lootPlays < 1)
            {
                Log.Warn($"Cannot play loot with {lootPlays} loot plays");
                return false;
            }

            var gameState = GameStateMachineManager.CurrentState;
            var theStackState = TheStackStateMachineManager.CurrentState;
            if (gameState != StateMachine.ActionPhaseIdling && theStackState != StateMachine.TheStackPriorityPassing)
            {
                Log.Warn($"Cannot play loot from {gameState} or {theStackState}");
                return false;
            }

            var hand = PlayerManager.ActivePlayer.Hand;
            if (!hand.Contains(cardId))
            {
                Log.Warn($"Cannot play loot {_cardConfig.GetName(cardId)} if hand does not contain it");
                return false;
            }

            return true;
        }
        protected override void Execute(int cardId)
        {
            GameStateMachineManager.SetTrigger(StateMachine.PlayLoot);
            GameManager.Singleton.PlayLoot(cardId);
        }
    }
}