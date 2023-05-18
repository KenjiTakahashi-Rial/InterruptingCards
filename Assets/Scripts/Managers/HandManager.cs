using System;

using Unity.Netcode;
using UnityEngine;
using EventType = Unity.Netcode.NetworkListEvent<int>.EventType;

using InterruptingCards.Behaviours;
using InterruptingCards.Config;

namespace InterruptingCards.Managers
{
    public class HandManager : NetworkBehaviour
    {
        private readonly CardConfig _cardConfig = CardConfig.Singleton;
        public NetworkList<int> _cardIds;

        [SerializeField] private CardBehaviour[] _cardSlots;

        private int Count => _cardIds.Count;

        public Action<int> OnCardClicked { get; set; }

        public int this[int i] => _cardIds[i];


        public void Awake()
        {
            _cardIds = new();

            for (var i = 0; i < _cardSlots.Length; i++)
            {
                var j = i;
                _cardSlots[i].OnClicked += () => OnCardClicked?.Invoke(j);
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                _cardIds.OnListChanged += HandleCardIdsChanged;
                SetCards(0, _cardSlots.Length);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                _cardIds.OnListChanged -= HandleCardIdsChanged;
            }

            base.OnNetworkDespawn();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var slot in _cardSlots)
            {
                slot.OnClicked = null;
            }
        }

        public void Add(int cardId)
        {
            Debug.Log($"Adding card to hand ({_cardConfig.GetCardString(cardId)})");

            if (Count == _cardSlots.Length)
            {
                throw new TooManyCardsException("Cannot add more cards than card slots");
            }

            _cardIds.Add(cardId);
        }

        public int RemoveAt(int i)
        {
            var cardId = _cardIds[i];
            _cardIds.RemoveAt(i);
            return cardId;
        }

        public void Clear()
        {
            Debug.Log("Clearing hand");

            _cardIds.Clear();
        }

        public bool Contains(int id)
        {
            return _cardIds.Contains(id);
        }

        private void SetCard(int i)
        {
            var cardSlot = _cardSlots[i];
            cardSlot.CardId = i >= Count ? CardConfig.InvalidId : _cardIds[i];
        }

        private void SetCards(int startInclusive, int endExclusive)
        {
            for (var i = startInclusive; i < endExclusive; i++)
            {
                SetCard(i);
            }
        }

        private void HandleCardIdsChanged(NetworkListEvent<int> changeEvent)
        {
            switch (changeEvent.Type)
            {
                case EventType.Add:
                    SetCard(Count - 1);
                    break;
                case EventType.Insert:
                case EventType.Remove:
                case EventType.RemoveAt:
                case EventType.Value:
                    SetCards(changeEvent.Index, _cardSlots.Length);
                    break;
                case EventType.Clear:
                case EventType.Full:
                    SetCards(0, _cardSlots.Length);
                    break;
            }
        }
    }
}
