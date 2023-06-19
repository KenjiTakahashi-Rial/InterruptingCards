using System.Linq;

using InterruptingCards.Config;
using InterruptingCards.Managers;

namespace InterruptingCards.Actions
{
    public class ActivateAbilityAction : AbstractCardAction
    {
        protected override bool CanExecute(ulong playerId, int cardId)
        {
            var priorityPlayer = PriorityManager.PriorityPlayer;
            if (playerId != priorityPlayer.Id)
            {
                Log.Warn($"Cannot activate ability if not priority player (priority player: {priorityPlayer.Name})");
                return false;
            }

            var gameState = GameStateMachineManager.CurrentState;
            var theStackState = TheStackStateMachineManager.CurrentState;
            if (gameState != StateMachine.ActionPhaseIdling && theStackState != StateMachine.TheStackPriorityPassing)
            {
                Log.Warn($"Cannot activate ability from {gameState} or {theStackState}");
                return false;
            }

            var activatedCards = PlayerManager.ActivePlayer.ActivatedCards;
            if (!activatedCards.Any(c => c.CardId == cardId))
            {
                Log.Warn($"Cannot activate ability that the palyer does not have");
                return false;
            }

            var card = activatedCards.Single(c => c.CardId == cardId);
            if (card.IsDeactivated)
            {
                Log.Warn($"Cannot activate ability that is deactivated");
                return false;
            }

            return true;
        }

        protected override void Execute(int cardId)
        {
            if (GameStateMachineManager.CurrentState == StateMachine.ActionPhaseIdling)
            {
                GameStateMachineManager.SetTrigger(StateMachine.ActivateAbility);
            }

            GameManager.Singleton.ActivateAbility(cardId);
        }
    }
}