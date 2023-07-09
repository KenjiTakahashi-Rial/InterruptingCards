using InterruptingCards.Config;

namespace InterruptingCards.Actions
{
    public class PurchaseAction : AbstractCardAction
    {
        protected override bool CanExecute(ulong playerId, int cardId)
        {
            var activePlayer = PlayerManager.ActivePlayer;
            if (playerId != activePlayer.Id)
            {
                Log.Warn($"Cannot purchase if not active player (active player: {PlayerManager.ActivePlayer.Name})");
                return false;
            }

            var purchases = activePlayer.Purchases;
            if (purchases < 1)
            {
                Log.Warn($"Cannot purchase with {purchases} purchases");
                return false;
            }

            var money = activePlayer.Money;
            var cost = activePlayer.PurchaseCost;
            if (money < cost)
            {
                Log.Warn($"Cannot purchase ({cost}) with {money}¢");
                return false;
            }

            var gameState = GameStateMachineManager.CurrentState;
            var theStackState = TheStackStateMachineManager.CurrentState;
            if (gameState != StateMachine.Purchasing || theStackState != StateMachine.TheStackIdling)
            {
                Log.Warn($"Cannot purchase from {gameState} and {theStackState}");
                return false;
            }

            var shop = Game.Shop;
            var treasureTopId = Game.TreasureDeck.TopCardId;
            if (!shop.Contains(cardId) && treasureTopId != cardId)
            {
                Log.Warn("Cannot purchase card not in shop or on top of treasure deck");
                return false;
            }

            return true;
        }

        protected override void Execute(int cardId)
        {
            var treasure = Game.TreasureDeck;
            var shop = Game.Shop;
            var player = PlayerManager.ActivePlayer;

            if (treasure.TopCardId == cardId)
            {
                treasure.DrawTop();
            }
            else
            {
                var index = shop.IndexOf(cardId);
                shop.Remove(cardId);
                shop.Insert(index, treasure.DrawTop());
            }

            player.Purchases--;
            // TODO: Add to player items

            GameStateMachineManager.SetTrigger(StateMachine.PurchaseComplete);
        }
    }
}