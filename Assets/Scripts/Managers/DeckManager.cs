using System;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Behaviours;
using InterruptingCards.Config;
using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public class DeckManager : NetworkBehaviour
    {
        private readonly DeckFactory _deckFactory = DeckFactory.Singleton;

        [SerializeField] private CardBehaviour _topCard;

        private Deck _deck;

        public event Action OnDeckClicked;
        
        public bool IsFaceUp
        {
            get => _topCard.IsFaceUp;
            set => _topCard.IsFaceUp = value;
        }

        public int Count => _deck.Count;

        public void Shuffle()
        {
            _deck.Shuffle();
        }

        public void PlaceTop(Card card)
        {
            _deck.PlaceTop(card);
        }

        public Card DrawTop()
        {
            var card = _deck.DrawTop();
            return card;
        }

        public void Initialize(CardPack cardPack)
        {
            _deck = _deckFactory.Create(cardPack);
            SetTop();
        }

        public void Clear()
        {
            _deck.Clear();
        }

        private void Awake()
        {
            _deck = _deckFactory.Create();
        }

        private void OnEnable()
        {
            _topCard.OnClicked -= InvokeOnDeckClicked;
            _deck.OnChanged -= SetTop;

            _topCard.OnClicked += InvokeOnDeckClicked;
            _deck.OnChanged += SetTop;
        }

        private void OnDisable()
        {
            _topCard.OnClicked -= InvokeOnDeckClicked;
            _deck.OnChanged -= SetTop;
        }

        private void SetTop()
        {
            _topCard.Card = _deck == null || _deck.Count == 0 ? null : _deck.PeekTop();
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
    }
}
