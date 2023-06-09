using System;
using System.Collections.Generic;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Managers;
using InterruptingCards.Models;

namespace InterruptingCards.Behaviours
{
    public class DeckBehaviour : NetworkBehaviour
    {
        private readonly CardConfig _cardConfig = CardConfig.Singleton;
        private List<int> _cardIds = new();

#pragma warning disable RCS1169 // Make field read-only.
        [SerializeField] private CardBehaviour _topCard;
#pragma warning restore RCS1169 // Make field read-only.

        public event Action OnClicked;

        public bool IsFaceUp
        {
            get => _topCard.IsFaceUp;
            set => _topCard.IsFaceUp = value;
        }

        private LogManager Log => LogManager.Singleton;

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

        public void SetHidden(bool val)
        {
            _topCard.SetHidden(val);
        }

        public void Shuffle()
        {
            CheckEmpty();
            Utilities.Functions.Shuffle(_cardIds);
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

        public void Initialize(Func<Card, bool> predicate = null)
        {
            _cardIds = _cardConfig.GenerateIdDeck(predicate);
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
                Log.Info("Deck OnClicked has no subscribers");
            }
            else
            {
                Log.Info("Deck clicked");
            }

            OnClicked?.Invoke();
        }
    }
}
