using InterruptingCards.Config;

namespace InterruptingCards.Actions
{
    public class DeclarePurchaseAction : AbstractAction
    {
        protected override bool CanExecute(ulong playerId)
        {
            if (playerId != PlayerManager.ActivePlayer.Id)
            {
                Log.Warn(
                    $"Cannot declare purchase if not active player (active player: {PlayerManager.ActivePlayer.Name})"
                );
                return false;
            }

            // TODO: Check for remaining purchases this turn

            var gameState = GameStateMachineManager.CurrentState;
            if (gameState != StateMachine.ActionPhaseIdling)
            {
                Log.Warn($"Cannot declare purchase from {gameState}");
                return false;
            }

            return true;
        }

        protected override void Execute()
        {
            // TODO
            GameStateMachineManager.SetTrigger(StateMachine.DeclarePurchase);
        }
    }
}