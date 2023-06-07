using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Managers;

namespace InterruptingCards.Actions
{
    public class ActivateAbilityAction : AbstractAction
    {
        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private StateMachineManager _gameStateMachineManager;

        protected override bool CanExecute
        {
            get
            {
                // TODO: Integrate stack and priority ability activation

                if (_playerManager.SelfId != _playerManager.ActivePlayer.Id)
                {
                    Debug.LogWarning(
                        $"Cannot activate ability if not active player (self: {_playerManager.SelfId}, active player: " +
                        $"{_playerManager.ActivePlayer.Name}"
                    );
                    return false;
                }

                if (_gameStateMachineManager.CurrentState != StateMachine.ActionPhaseIdling)
                {
                    Debug.LogWarning($"Cannot activate ability from {_gameStateMachineManager.CurrentState}");
                    return false;
                }

                return true;
            }
        }

        protected override void Execute()
        {
            // TODO
            _gameStateMachineManager.SetTrigger(StateMachine.ActivateAbility);
            GameManager.Singleton.ActivateAbility();
        }
    }
}