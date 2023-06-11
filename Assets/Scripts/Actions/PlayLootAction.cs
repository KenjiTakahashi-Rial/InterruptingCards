using InterruptingCards.Config;
using InterruptingCards.Managers;

namespace InterruptingCards.Actions
{
    public class PlayLootAction : AbstractCardAction
    {
        protected override bool CanExecute(ulong playerId, int cardId)
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
        protected override void Execute(int cardId)
        {
            // TODO
            _gameStateMachineManager.SetTrigger(StateMachine.PlayLoot);
            GameManager.Singleton.PlayLoot();
        }
    }
}