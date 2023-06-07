using UnityEngine;

using InterruptingCards.Config;

namespace InterruptingCards.Actions
{
    public class AttackAction : AbstractAction
    {
        protected override bool CanExecute(ulong playerId)
        {
            if (playerId != _playerManager.ActivePlayer.Id)
            {
                Debug.LogWarning(
                    $"Cannot attack if not active player (self: {_playerManager.SelfId}, active player: " +
                    $"{_playerManager.ActivePlayer.Name})"
                );
                return false;
            }

            if (_gameStateMachineManager.CurrentState != StateMachine.Attacking)
            {
                Debug.LogWarning($"Cannot attack from {_gameStateMachineManager.CurrentState}");
                return false;
            }

            return true;
        }

        protected override void Execute()
        {
            // TODO
            _gameStateMachineManager.SetTrigger(StateMachine.AttackComplete);
        }
    }
}