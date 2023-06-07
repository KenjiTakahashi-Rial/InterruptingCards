using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Managers;

namespace InterruptingCards.Actions
{
    public class PurchaseAction : AbstractAction
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
                        $"Cannot purchase if not active player (self: {_playerManager.SelfId}, active player: " +
                        $"{_playerManager.ActivePlayer.Name}"
                    );
                    return false;
                }

                if (_gameStateMachineManager.CurrentState != StateMachine.Purchasing)
                {
                    Debug.LogWarning($"Cannot purchase from {_gameStateMachineManager.CurrentState}");
                    return false;
                }

                return true;
            }
        }

        protected override void Execute()
        {
            // TODO
            _gameStateMachineManager.SetTrigger(StateMachine.PurchaseComplete);
        }
    }
}