using InterruptingCards.Config;

namespace InterruptingCards.Actions
{
    public class DeclareAttackAction : AbstractAction
    {
        protected override bool CanExecute(ulong playerId)
        {
            if (playerId != _playerManager.ActivePlayer.Id)
            {
                Log.Warn(
                    $"Cannot declare attack if not active player (active player: {_playerManager.ActivePlayer.Id})"
                );
                return false;
            }

            // TODO: Check for number of attacks remaining this turn

            var gameState = _gameStateMachineManager.CurrentState;
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
            _gameStateMachineManager.SetTrigger(StateMachine.DeclareAttack);
        }
    }
}