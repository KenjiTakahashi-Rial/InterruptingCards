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

            if (_gameStateMachineManager.CurrentState != StateMachine.ActionPhaseIdling)
            {
                Debug.LogWarning($"Cannot declare purchase from {_gameStateMachineManager.CurrentState}");
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