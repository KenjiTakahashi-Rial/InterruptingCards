using InterruptingCards.Config;

namespace InterruptingCards.Actions
{
    public class DeclareAttackAction : AbstractAction
    {
        protected override bool CanExecute(ulong playerId)
        {
            if (playerId != PlayerManager.ActivePlayer.Id)
            {
                Log.Warn(
                    $"Cannot declare attack if not active player (active player: {PlayerManager.ActivePlayer.Id})"
                );
                return false;
            }

            // TODO: Check for number of attacks remaining this turn

            var gameState = GameStateMachineManager.CurrentState;
            if (gameState != StateMachine.ActionPhaseIdling)
            {
                Log.Warn($"Cannot declare attack from {gameState}");
                return false;
            }

            return true;
        }

        protected override void Execute()
        {
            // TODO
            GameStateMachineManager.SetTrigger(StateMachine.DeclareAttack);
        }
    }
}