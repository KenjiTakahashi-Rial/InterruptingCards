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

        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private TheStackManager _theStackManager;

        [SerializeField] private DeckBehaviour _lootDiscard;

        private LogManager Log => LogManager.Singleton;

        public void Resolve(TheStackElement element)
        {
            if (element.Value == CardConfig.InvalidId)
            {
                Log.Warn($"The Stack resolved an invalid loot");
            }

            var card = _cardConfig[element.Value];

            switch (card.PlayedEffect)
            {
                case CardPlayedEffect.GainCents:
                    _playerManager[element.PushedById].Money += (uint)card.Value;
                    break;
                default:
                    throw new NotImplementedException();
            }

            _lootDiscard.PlaceTop(card.Id);
        }
    }
}