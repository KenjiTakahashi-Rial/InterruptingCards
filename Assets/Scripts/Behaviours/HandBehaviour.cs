using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Managers;
using System.Collections;

namespace InterruptingCards.Behaviours
{
    public class HandBehaviour : NetworkBehaviour, IEnumerable<CardBehaviour>
    {
        private readonly CardConfig _cardConfig = CardConfig.Singleton;

        [SerializeField] private CardBehaviour[] _cardSlots;

        public int Count => _cardSlots.Count(c => c.CardId != CardConfig.InvalidId);

        public Action<int> OnCardClicked { get; set; }

        private LogManager Log => LogManager.Singleton;

        public int this[int i] => _cardSlots[i].CardId;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _cardSlots.GetEnumerator();
        }

        public IEnumerator<CardBehaviour> GetEnumerator()
        {
            return ((IEnumerable<CardBehaviour>)_cardSlots).GetEnumerator();
        }

        public void Awake()
        {
            for (var i = 0; i < _cardSlots.Length; i++)
            {
                var card = _cardSlots[i];
                card.OnClicked += () => OnCardClicked?.Invoke(card.CardId);
            }
        }

        public override void OnDestroy()
        {
            foreach (var slot in _cardSlots)
            {
                slot.OnClicked = null;
            }

            base.OnDestroy();
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
            Log.Info($"Adding card to hand ({_cardConfig.GetName(cardId)})");

            if (Count == _cardSlots.Length)
            {
                throw new TooManyCardsException("Cannot add more cards than card slots");
            }

            _cardSlots[Count].CardId = cardId;
        }

        public int Remove(int cardId)
        {
            Log.Info($"Removing {_cardConfig.GetName(cardId)} from hand");

            var index = Array.FindIndex(_cardSlots, s => s.CardId == cardId);
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
                slot.CardId = CardConfig.InvalidId;
            }
        }

        public bool Contains(int cardId)
        {
            foreach (var slot in _cardSlots)
            {
                if (slot.CardId == cardId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
