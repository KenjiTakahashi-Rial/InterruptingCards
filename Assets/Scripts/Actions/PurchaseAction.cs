using InterruptingCards.Config;

namespace InterruptingCards.Actions
{
    public class PurchaseAction : AbstractCardAction
    {
        protected override bool CanExecute(ulong playerId, int cardId)
        {
            if (playerId != _playerManager.ActivePlayer.Id)
            {
                Log.Warn($"Cannot purchase if not active player (active player: {_playerManager.ActivePlayer.Name})");
                return false;
            }

            var gameState = _gameStateMachineManager.CurrentState;
            if (gameState != StateMachine.Purchasing)
            {
                Log.Warn($"Cannot purchase from {gameState}");
                return false;
            }

            return true;
        }

        protected override void Execute(int cardId)
        {
            // TODO
            _gameStateMachineManager.SetTrigger(StateMachine.PurchaseComplete);
        }
    }
}