using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Managers;

namespace InterruptingCards.Actions
{
    public class PlayLootAction : AbstractAction
    {
        protected override bool CanExecute(ulong playerId)
        {
            // TODO: Integrate stack and priority loot plays

            if (playerId != _playerManager.ActivePlayer.Id)
            {
                Debug.LogWarning(
                    $"Cannot play loot if not active player (self: {_playerManager.SelfId}, active player: " +
                    $"{_playerManager.ActivePlayer.Name} )"
                );
                return false;
            }

            if (_gameStateMachineManager.CurrentState != StateMachine.ActionPhaseIdling)
            {
                Debug.LogWarning($"Cannot play loot from {_gameStateMachineManager.CurrentState}");
                return false;
            }

            return true;
        }
        protected override void Execute()
        {
            // TODO
            _gameStateMachineManager.SetTrigger(StateMachine.PlayLoot);
            GameManager.Singleton.PlayLoot();
        }
    }
}