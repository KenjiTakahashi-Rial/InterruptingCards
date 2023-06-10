using InterruptingCards.Config;

namespace InterruptingCards.Actions
{
    public class DeclareEndTurnAction : AbstractAction
    {
        protected override bool CanExecute(ulong playerId)
        {
            if (playerId != _playerManager.ActivePlayer.Id)
            {
                Log.Warn($"Cannot end turn if not active player (active player: {_playerManager.ActivePlayer.Name})");
                return false;
            }

            var gameState = _gameStateMachineManager.CurrentState;
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
            _gameStateMachineManager.SetTrigger(StateMachine.DeclareEndTurn);
        }
    }
}