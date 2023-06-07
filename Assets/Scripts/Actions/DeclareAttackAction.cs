using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Managers;

namespace InterruptingCards.Actions
{
    public class DeclareAttackAction : AbstractAction
    {
        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private StateMachineManager _gameStateMachineManager;

        protected override bool CanExecute
        {
            get
            {
                if (_playerManager.SelfId != _playerManager.ActivePlayer.Id)
                {
                    Debug.LogWarning(
                        $"Cannot declare attack if not active player (self: {_playerManager.SelfId}, active player: " +
                        $"{_playerManager.ActivePlayer.Id}"
                    );
                    return false;
                }

                // TODO: Check for number of attacks remaining this turn

                if (_gameStateMachineManager.CurrentState != StateMachine.ActionPhaseIdling)
                {
                    Debug.LogWarning($"Cannot declare attack from {_gameStateMachineManager.CurrentState}");
                    return false;
                }

                return true;
            }
        }

        protected override void Execute()
        {
            // TODO
            _gameStateMachineManager.SetTrigger(StateMachine.DeclareAttack);
        }
    }
}