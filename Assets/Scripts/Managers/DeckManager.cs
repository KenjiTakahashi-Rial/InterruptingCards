using System;
using System.Collections.Generic;
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
        private List<int> _cardIds = new();

        [SerializeField] private CardBehaviour _topCard;

        public event Action OnClicked;
        
        public bool IsFaceUp
        {
            get => _topCard.IsFaceUp;
            set => _topCard.IsFaceUp = value;
        }

        private int TopIndex => _cardIds.Count - 1;

        public void Awake()
        {
            _topCard.OnClicked += InvokeOnClicked;
        }

        public override void OnDestroy()
        {
            _topCard.OnClicked -= InvokeOnClicked;
            base.OnDestroy();
        }

        public void Shuffle()
        {
            CheckEmpty();
            _cardIds = _cardIds.OrderBy(x => Guid.NewGuid()).ToList();
            SetTop();
        }

        public void PlaceTop(int cardId)
        {
            _cardIds.Add(cardId);
            SetTop();
        }

        public int DrawTop()
        {
            var cardId = _cardIds[TopIndex];
            _cardIds.RemoveAt(TopIndex);
            SetTop();
            return cardId;
        }

        public void Initialize()
        {
            _cardIds = _cardConfig.GenerateDeck();
            SetTop();
        }

        public void Clear()
        {
            _cardIds.Clear();
            SetTop();
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

        private void InvokeOnClicked()
        {
            if (OnClicked == null)
            {
                Debug.Log("Deck OnClicked has no subscribers");
            }
            else
            {
                Debug.Log("Deck clicked");
            }

            OnClicked?.Invoke();
        }
    }
}
