using InterruptingCards.Config;
using InterruptingCards.Managers;
using InterruptingCards.Managers.TheStack;

namespace InterruptingCards.Actions
{
    public class PlayLootAction : AbstractCardAction
    {
        protected override bool CanExecute(ulong playerId, int cardId)
        {
            var priorityPlayer = PriorityManager.PriorityPlayer;
            if (playerId != priorityPlayer.Id)
            {
                Log.Warn($"Cannot play loot without priority (priority player: {priorityPlayer.Name})");
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

            var hand = priorityPlayer.Hand;
            if (!hand.Contains(cardId))
            {
                Log.Warn($"Cannot play loot {_cardConfig.GetName(cardId)} if hand does not contain it");
                return false;
            }

            return true;
        }
        protected override void Execute(int cardId)
        {
            var player = PriorityManager.PriorityPlayer;
            player.LootPlays--;
            player.Hand.Remove(cardId);
            TheStackManager.PushLoot(player, cardId);

            var isActive = player == PlayerManager.ActivePlayer;
            var isActionPhaseIdling = GameStateMachineManager.CurrentState == StateMachine.ActionPhaseIdling;
            if (isActive && isActionPhaseIdling)
            {
                GameStateMachineManager.SetTrigger(StateMachine.PlayLoot);
            }
        }
    }
}