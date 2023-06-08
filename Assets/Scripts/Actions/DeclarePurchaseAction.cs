using UnityEngine;

using InterruptingCards.Config;

namespace InterruptingCards.Actions
{
    public class DeclarePurchaseAction : AbstractAction
    {
        protected override bool CanExecute(ulong playerId)
        {
            if (playerId != _playerManager.ActivePlayer.Id)
            {
                Debug.LogWarning(
                    $"Cannot declare purchase if not active player (self: {_playerManager.SelfId}, active player: " +
                    $"{_playerManager.ActivePlayer.Name})"
                );
                return false;
            }

            // TODO: Check for remaining purchases this turn

            var gameState = _gameStateMachineManager.CurrentState;
            if (gameState != StateMachine.ActionPhaseIdling)
            {
                Debug.LogWarning($"Cannot declare purchase from {gameState}");
                return false;
            }

            return true;
        }

        protected override void Execute()
        {
            // TODO
            _gameStateMachineManager.SetTrigger(StateMachine.DeclarePurchase);
        }
    }
}