using InterruptingCards.Config;
using InterruptingCards.Managers;

namespace InterruptingCards.Actions
{
    public class ActivateAbilityAction : AbstractAction
    {
        protected override bool CanExecute(ulong playerId)
        {
            // TODO: Integrate stack and priority ability activation

            if (playerId != _playerManager.ActivePlayer.Id)
            {
                Log.Warn(
                    $"Cannot activate ability if not active player (active player: {_playerManager.ActivePlayer.Name})"
                );
                return false;
            }

            var gameState = _gameStateMachineManager.CurrentState;
            if (gameState != StateMachine.ActionPhaseIdling)
            {
                Log.Warn($"Cannot activate ability from {gameState}");
                return false;
            }

            return true;
        }

        protected override void Execute()
        {
            // TODO
            _gameStateMachineManager.SetTrigger(StateMachine.ActivateAbility);
            GameManager.Singleton.ActivateAbility();
        }
    }
}