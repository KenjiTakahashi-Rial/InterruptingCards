using UnityEngine;

using InterruptingCards.Config;

namespace InterruptingCards.Actions
{
    public class DeclareAttackAction : AbstractAction
    {
        protected override bool CanExecute(ulong playerId)
        {
            if (playerId != _playerManager.ActivePlayer.Id)
            {
                Debug.LogWarning(
                    $"Cannot declare attack if not active player (self: {_playerManager.SelfId}, active player: " +
                    $"{_playerManager.ActivePlayer.Id})"
                );
                return false;
            }

            // TODO: Check for number of attacks remaining this turn

            var gameState = _gameStateMachineManager.CurrentState;
            if (gameState != StateMachine.ActionPhaseIdling)
            {
                Debug.LogWarning($"Cannot declare attack from {gameState}");
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