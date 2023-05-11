using System;

using TMPro;
using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Factories;
using InterruptingCards.Models;

namespace InterruptingCards.Behaviours
{
    public class BasicCardBehaviour : NetworkBehaviour, ICardBehaviour<BasicCard>
    {
        protected readonly NetworkVariable<bool> _isFaceUp = new(true);
        protected readonly NetworkVariable<int> _cardId = new(CardConfig.InvalidId);

        [SerializeField] protected TextMeshPro _cardText;
        [SerializeField] protected SpriteRenderer _cardSprite;
        [SerializeField] protected CardSuit _startingSuit;
        [SerializeField] protected CardRank _startingRank;

        protected bool _offlineIsFaceUp;
        protected BasicCard _card;
        protected Vector3 _originalScale;

        public event Action OnClicked;

        public event Action OnValueChanged;

        public virtual bool IsFaceUp
        {
            get => NetworkManager != null && NetworkManager.IsListening ? _isFaceUp.Value : _offlineIsFaceUp;
            set
            {
                if (NetworkManager != null && NetworkManager.IsListening)
                {
                    _isFaceUp.Value = value;
                }

                _offlineIsFaceUp = value;
                Refresh();
            }
        }

        public virtual BasicCard Card
        {
            get => _card;
            set
            {
                _card = value;

                if (NetworkManager != null && NetworkManager.IsListening)
                {
                    _cardId.Value = value == null ? CardConfig.InvalidId : value.Id;
                }

                Refresh();
            }
        }

        protected virtual int StartingCardId => CardConfig.GetCardId(_startingSuit, _startingRank);

        protected virtual ICardFactory<BasicCard> CardFactory => BasicCardFactory.Singleton;

        public override void OnNetworkSpawn()
        {
            _cardId.OnValueChanged -= HandleCardIdChanged;
            _cardId.OnValueChanged += HandleCardIdChanged;

            _isFaceUp.OnValueChanged -= HandleFaceUpChanged;
            _isFaceUp.OnValueChanged += HandleFaceUpChanged;
        }

        public override void OnNetworkDespawn()
        {
            _cardId.OnValueChanged -= HandleCardIdChanged;
            _isFaceUp.OnValueChanged -= HandleFaceUpChanged;
        }

        public void UnsubscribeAllOnClicked()
        {
            OnClicked = null;
        }

        public void UnsubscribeAllOnValueChanged()
        {
            OnValueChanged = null;
        }

        protected void HandleCardIdChanged(int oldValue, int newValue)
        {
            Debug.Log($"Card changed ({oldValue} -> {newValue})");

            if (OnValueChanged == null)
            {
                Debug.Log("OnValueChanged has no subscribers");
            }

            _card = newValue == CardConfig.InvalidId ? null : CardFactory.Create(newValue);

            OnValueChanged?.Invoke();
            Refresh();
        }

        protected void HandleFaceUpChanged(bool oldValue, bool newValue)
        {
            var before = oldValue ? "face-up" : "face-down";
            var after = newValue ? "face-up" : "face-down";
            Debug.Log($"Card changed ({before} -> {after})");

            Refresh();
        }

        protected virtual void Refresh()
        {
            if (Card == null)
            {
                _cardSprite.enabled = false;
                _cardText.enabled = false;
            }
            else if (!IsFaceUp)
            {
                _cardSprite.enabled = true;
                _cardText.enabled = false;
            }
            else
            {
                _cardText.SetText(Card.ToString()); // TODO: Change
                _cardSprite.enabled = true;
                _cardText.enabled = true;
            }
        }

        protected virtual void Awake()
        {
            _originalScale = transform.localScale;

            if (_startingSuit != CardSuit.Invalid && _startingRank != CardRank.Invalid)
            {
                Card = CardFactory.Create(StartingCardId);
            }
        }

        protected void Update()
        {
            // TODO: NetworkManager spawns the objects at wrong scale. Figure out why and find a better solution
            if (transform.localScale != _originalScale)
            {
                transform.localScale = _originalScale;
            }
        }

        protected virtual void OnMouseDown()
        {
            if (OnClicked == null)
            {
                Debug.Log("OnClicked has no subscribers");
            }
            else
            {
                Debug.Log("Mouse down on card");
            }

            OnClicked?.Invoke();
        }
    }
}