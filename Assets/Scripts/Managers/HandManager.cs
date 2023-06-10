using System;
using System.Linq;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Behaviours;
using InterruptingCards.Config;

namespace InterruptingCards.Managers
{
    public class HandManager : NetworkBehaviour
    {
        private readonly CardConfig _cardConfig = CardConfig.Singleton;

        [SerializeField] private CardBehaviour[] _cardSlots;

        public int Count => _cardSlots.Count(c => c.CardId != CardConfig.InvalidId);

        public Action<int> OnCardClicked { get; set; }

        private LogManager Log => LogManager.Singleton;

        public int this[int i] => _cardSlots[i].CardId;


        public void Awake()
        {
            for (var i = 0; i < _cardSlots.Length; i++)
            {
                var j = i;
                _cardSlots[i].OnClicked += () => OnCardClicked?.Invoke(j);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var slot in _cardSlots)
            {
                slot.OnClicked = null;
            }
        }
        public void SetHidden(bool val)
        {
            foreach (var card in _cardSlots)
            {
                card.SetHidden(val);
            }
        }

        public void Add(int cardId)
        {
            Log.Info($"Adding card to hand ({_cardConfig.GetCardString(cardId)})");

            if (Count == _cardSlots.Length)
            {
                throw new TooManyCardsException("Cannot add more cards than card slots");
            }

            _cardSlots[Count].CardId = cardId;
        }

        public int RemoveAt(int index)
        {
            var cardId = _cardSlots[index].CardId;

            Log.Info($"Removing {_cardConfig.GetCardString(cardId)} from hand index {index}");

            for (int i = index; i < _cardSlots.Length; i++)
            {
                var nextId = i == _cardSlots.Length - 1 ? CardConfig.InvalidId : _cardSlots[i + 1].CardId;
                _cardSlots[i].CardId = nextId;

                if (nextId == CardConfig.InvalidId)
                {
                    break;
                }
            }

            return cardId;
        }

        public void Clear()
        {
            Log.Info("Clearing hand");

            foreach (var slot in _cardSlots)
            {
                if (slot.CardId == CardConfig.InvalidId)
                {
                    break;
                }
                
                slot.CardId = CardConfig.InvalidId;
            }
        }
    }
}
