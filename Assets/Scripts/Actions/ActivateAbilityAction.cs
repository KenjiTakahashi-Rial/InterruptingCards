using InterruptingCards.Config;
using InterruptingCards.Managers;

namespace InterruptingCards.Actions
{
    public class ActivateAbilityAction : AbstractCardAction
    {
        protected override bool CanExecute(ulong playerId, int cardId)
        {
            // TODO: Integrate stack and priority ability activation

            if (playerId != PlayerManager.ActivePlayer.Id)
            {
                Log.Warn(
                    $"Cannot activate ability if not active player (active player: {PlayerManager.ActivePlayer.Name})"
                );
                return false;
            }

            var gameState = GameStateMachineManager.CurrentState;
            if (gameState != StateMachine.ActionPhaseIdling)
            {
                Log.Warn($"Cannot activate ability from {gameState}");
                return false;
            }

            var activatedCards = PlayerManager.ActivePlayer.ActivatedCards;
            if (!activatedCards.ContainsKey(cardId))
            {
                Log.Warn($"Cannot activate ability that the palyer does not have");
                return false;
            }

            if (activatedCards[cardId].IsDeactivated)
            {
                Log.Warn($"Cannot activate ability that has already been activated");
                return false;
            }

            return true;
        }

        protected override void Execute(int cardId)
        {
            // TODO
            GameStateMachineManager.SetTrigger(StateMachine.ActivateAbility);
            GameManager.Singleton.ActivateAbility(cardId);
        }
    }
}