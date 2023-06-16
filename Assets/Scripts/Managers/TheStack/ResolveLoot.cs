using System;

using UnityEngine;

using InterruptingCards.Behaviours;
using InterruptingCards.Config;
using InterruptingCards.Models;

namespace InterruptingCards.Managers.TheStack
{
    public class ResolveLoot : MonoBehaviour
    {
        private readonly CardConfig _cardConfig = CardConfig.Singleton;

        private GameManager Game => GameManager.Singleton;
        private LogManager Log => LogManager.Singleton;
        private PlayerManager PlayerManager => Game.PlayerManager;

        private DeckBehaviour LootDiscard => Game.LootDiscard;

        public void Resolve(TheStackElement element)
        {
            if (element.Value == CardConfig.InvalidId)
            {
                Log.Warn($"The Stack resolved an invalid loot");
            }

            var card = _cardConfig[element.Value];

            switch (card.LootAbility)
            {
                case CardAbility.GainCents:
                    PlayerManager[element.PushedById].Money += (uint)card.Value;
                    break;
                default:
                    throw new NotImplementedException($"Resolution for {card.LootAbility} not implemented");
            }

            LootDiscard.PlaceTop(card.Id);
        }
    }
}