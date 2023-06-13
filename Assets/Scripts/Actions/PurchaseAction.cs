using InterruptingCards.Config;

namespace InterruptingCards.Actions
{
    public class PurchaseAction : AbstractCardAction
    {
        protected override bool CanExecute(ulong playerId, int cardId)
        {
            if (playerId != PlayerManager.ActivePlayer.Id)
            {
                Log.Warn($"Cannot purchase if not active player (active player: {PlayerManager.ActivePlayer.Name})");
                return false;
            }

            var gameState = GameStateMachineManager.CurrentState;
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
            GameStateMachineManager.SetTrigger(StateMachine.PurchaseComplete);
        }
    }
}