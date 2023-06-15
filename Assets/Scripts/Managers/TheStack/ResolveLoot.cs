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
                case Ability.GainCents:
                    PlayerManager[element.PushedById].Money += (uint)card.Value;
                    break;
                default:
                    throw new NotImplementedException();
            }

            LootDiscard.PlaceTop(card.Id);
        }
    }
}