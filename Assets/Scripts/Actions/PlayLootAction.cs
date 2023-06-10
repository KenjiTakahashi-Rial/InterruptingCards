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
                Log.Warn($"Cannot play loot if not active player (active player: {_playerManager.ActivePlayer.Name})");
                return false;
            }

            var gameState = _gameStateMachineManager.CurrentState;
            if (gameState != StateMachine.ActionPhaseIdling)
            {
                Log.Warn($"Cannot play loot from {gameState}");
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