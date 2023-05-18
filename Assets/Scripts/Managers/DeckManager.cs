using System;
using System.Linq;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Behaviours;
using InterruptingCards.Config;

namespace InterruptingCards.Managers
{
    public class DeckManager : NetworkBehaviour
    {
        private readonly CardConfig _cardConfig = CardConfig.Singleton;
        public NetworkList<int> _cardIds;

        [SerializeField] private CardBehaviour _topCard;

        public event Action OnDeckClicked;
        
        public bool IsFaceUp
        {
            get => _topCard.IsFaceUp;
            set => _topCard.IsFaceUp = value;
        }

        private int TopIndex => _cardIds.Count - 1;

        public void Awake()
        {
            _cardIds = new();
            _topCard.OnClicked += InvokeOnDeckClicked;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                _cardIds.OnListChanged += HandleCardIdsChanged;
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
            _topCard.OnClicked -= InvokeOnDeckClicked;
            base.OnDestroy();
        }

        public void Shuffle()
        {
            CheckEmpty();

            var cards = new int[_cardIds.Count];

            for (var i = 0; i < _cardIds.Count; i++)
            {
                cards[i] = _cardIds[i];
            }

            var shuffled = cards.OrderBy(x => Guid.NewGuid());
            _cardIds.Clear();

            foreach (var id in shuffled)
            {
                _cardIds.Add(id);
            }
        }

        public void PlaceTop(int cardId)
        {
            _cardIds.Insert(TopIndex + 1, cardId);
        }

        public int DrawTop()
        {
            var cardId = _cardIds[TopIndex];
            _cardIds.RemoveAt(TopIndex);
            return cardId;
        }

        public void Initialize()
        {
            var deck = _cardConfig.GenerateDeck();

            foreach (var id in deck)
            {
                _cardIds.Add(id);
            }
        }

        public void Clear()
        {
            _cardIds.Clear();
        }

        private void CheckEmpty()
        {
            if (_cardIds.Count == 0)
            {
                throw new CardCollectionEmptyException();
            }
        }

        private void SetTop()
        {
            _topCard.CardId = _cardIds.Count == 0 ? CardConfig.InvalidId : _cardIds[TopIndex];
        }

        private void InvokeOnDeckClicked()
        {
            if (OnDeckClicked == null)
            {
                Debug.Log("OnDeckClicked has no subscribers");
            }
            else
            {
                Debug.Log("Deck clicked");
            }

            OnDeckClicked?.Invoke();
        }

        private void HandleCardIdsChanged(NetworkListEvent<int> changeEvent)
        {
            SetTop();
        }
    }
}
