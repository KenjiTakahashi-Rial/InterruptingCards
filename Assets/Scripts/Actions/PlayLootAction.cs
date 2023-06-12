using InterruptingCards.Config;
using InterruptingCards.Managers;

namespace InterruptingCards.Actions
{
    public class PlayLootAction : AbstractCardAction
    {
        private readonly CardConfig _cardConfig = CardConfig.Singleton;

        protected override bool CanExecute(ulong playerId, int cardId)
        {
            // TODO: Integrate The Stack and priority loot plays (outside of active player's loot plays)
            // TODO: Check the number of loot plays the player has

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

            var hand = _playerManager.ActivePlayer.Hand;
            if (!hand.Contains(cardId))
            {
                Log.Warn($"Cannot play loot {_cardConfig.GetName(cardId)} if hand does not contain it");
                return false;
            }

            return true;
        }
        protected override void Execute(int cardId)
        {
            _gameStateMachineManager.SetTrigger(StateMachine.PlayLoot);
            GameManager.Singleton.PlayLoot(cardId);
        }
    }
}