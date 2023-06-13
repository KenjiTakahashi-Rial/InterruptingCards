using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Managers;

namespace InterruptingCards.Actions
{
    public class PlayLootAction : AbstractCardAction
    {
        private readonly CardConfig _cardConfig = CardConfig.Singleton;

        [SerializeField] private PriorityManager _priorityManager;
        [SerializeField] private StateMachineManager _theStackStateMachineManager;

        protected override bool CanExecute(ulong playerId, int cardId)
        {
            var priorityPlayer = _priorityManager.PriorityPlayer;

            if (playerId != priorityPlayer.Id)
            {
                Log.Warn(
                    $"Cannot play loot if not priority player (priority player: " +
                    $"{_priorityManager.PriorityPlayer.Name})");
                return false;
            }

            var lootPlays = priorityPlayer.LootPlays;
            if (lootPlays < 1)
            {
                Log.Warn($"Cannot play loot with {lootPlays} loot plays");
                return false;
            }

            var gameState = _gameStateMachineManager.CurrentState;
            var theStackState = _theStackStateMachineManager.CurrentState;
            if (gameState != StateMachine.ActionPhaseIdling && theStackState != StateMachine.TheStackPriorityPassing)
            {
                Log.Warn($"Cannot play loot from {gameState} or {theStackState}");
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