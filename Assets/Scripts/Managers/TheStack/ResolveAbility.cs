using System;

using UnityEngine;

using InterruptingCards.Models;

namespace InterruptingCards.Managers.TheStack
{
    public class ResolveAbility : MonoBehaviour
    {
        private GameManager Game => GameManager.Singleton;
        private LogManager Log => LogManager.Singleton;
        private PlayerManager PlayerManager => Game.PlayerManager;

        public void Resolve(TheStackElement element)
        {
            var ability = (CardAbility)element.Value;

            if (ability == CardAbility.Invalid)
            {
                Log.Warn("The Stack resolved an invalid ability");
            }

            switch (ability)
            {
                case CardAbility.AddLootPlay:
                    PlayerManager[element.PushedById].LootPlays++;
                    break;
                default:
                    throw new NotImplementedException($"Resolution for {ability} not implemented");
            }
        }
    }
}