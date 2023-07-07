using InterruptingCards.Config;

namespace InterruptingCards.Actions
{
    public class DeclareEndTurnAction : AbstractAction
    {
        protected override bool CanExecute(ulong playerId)
        {
            if (playerId != PlayerManager.ActivePlayer.Id)
            {
                Log.Warn($"Cannot end turn if not active player (active player: {PlayerManager.ActivePlayer.Name})");
                return false;
            }

            var gameState = GameStateMachineManager.CurrentState;
            if (gameState!= StateMachine.ActionPhaseIdling)
            {
                Log.Warn($"Cannot end turn from {gameState}");
                return false;
            }

            return true;
        }

        protected override void Execute()
        {
            // TODO
            GameStateMachineManager.SetTrigger(StateMachine.DeclareEndTurn);
        }
    }
}