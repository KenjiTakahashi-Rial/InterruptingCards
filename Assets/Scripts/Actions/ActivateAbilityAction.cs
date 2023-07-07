using System.Linq;

using InterruptingCards.Config;
using InterruptingCards.Managers;
using InterruptingCards.Managers.TheStack;

namespace InterruptingCards.Actions
{
    public class ActivateAbilityAction : AbstractCardAction
    {
        protected override bool CanExecute(ulong playerId, int cardId)
        {
            var priorityPlayer = PriorityManager.PriorityPlayer;
            if (playerId != priorityPlayer.Id)
            {
                Log.Warn($"Cannot activate ability without priority (priority player: {priorityPlayer.Name})");
                return false;
            }

            var gameState = GameStateMachineManager.CurrentState;
            var theStackState = TheStackStateMachineManager.CurrentState;
            if (gameState != StateMachine.ActionPhaseIdling && theStackState != StateMachine.TheStackPriorityPassing)
            {
                Log.Warn($"Cannot activate ability from {gameState} or {theStackState}");
                return false;
            }

            var activatedCards = priorityPlayer.ActivatedCards;
            if (!activatedCards.Any(c => c.CardId == cardId))
            {
                Log.Warn("Cannot activate ability that the player does not have");
                return false;
            }

            var card = activatedCards.Single(c => c.CardId == cardId);
            if (card.IsDeactivated)
            {
                Log.Warn("Cannot activate ability that is deactivated");
                return false;
            }

            return true;
        }

        protected override void Execute(int cardId)
        {
            var player = PriorityManager.PriorityPlayer;
            var cardBehaviour = player.ActivatedCards.Single(c => c.CardId == cardId);
            cardBehaviour.IsDeactivated = true;
            var card = _cardConfig[cardBehaviour.CardId];
            TheStackManager.PushAbility(player, card.ActivatedAbility);

            var isActive = player == PlayerManager.ActivePlayer;
            var isActionPhaseIdling = GameStateMachineManager.CurrentState == StateMachine.ActionPhaseIdling;
            if (isActive && isActionPhaseIdling)
            {
                GameStateMachineManager.SetTrigger(StateMachine.ActivateAbility);
            }
        }
    }
}