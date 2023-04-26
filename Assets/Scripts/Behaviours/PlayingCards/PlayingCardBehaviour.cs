using System;

using TMPro;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Models;

namespace InterruptingCards.Behaviours
{
    // TODO: Consider adding a network dependency for this so it can inherit most of the functionality from a generic abstract class
    public class PlayingCardBehaviour : NetworkBehaviour, ICardBehaviour<PlayingCardSuit, PlayingCardRank>
    {
        [SerializeField] private TextMeshPro _cardText;

        private readonly NetworkVariable<bool> _isFaceUp = new(true);
        private readonly NetworkVariable<ICard<PlayingCardSuit, PlayingCardRank>> _card = new();

        public event Action OnCardClicked;

        public bool IsFaceUp
        {
            get { return _isFaceUp.Value; }
            set
            {
                _isFaceUp.Value = value;
                Refresh();
            }
        }

        public ICard<PlayingCardSuit, PlayingCardRank> Card
        {
            get { return _card.Value; }
            set
            {
                _card.Value = value;
                Refresh();
            }
        }

        public override void OnNetworkSpawn()
        {
            _card.OnValueChanged -= Refresh;
            _card.OnValueChanged += Refresh;
            _isFaceUp.OnValueChanged -= Refresh;
            _isFaceUp.OnValueChanged += Refresh;
        }

        public override void OnNetworkDespawn()
        {
            _card.OnValueChanged -= Refresh;
            _isFaceUp.OnValueChanged -= Refresh;
        }

        public void UnsubscribeAllOnCardClicked()
        {
            OnCardClicked = null;
        }

        private void Refresh()
        {
            if (Card == null)
            {
                _cardText.gameObject.SetActive(false);
                return;
            }

            if (!IsFaceUp)
            {
                _cardText.enabled = false;
                return;
            }

            _cardText.SetText(_card.Value.ToString()); // TODO: Change  this
            _cardText.enabled = true;
        }

        private void Refresh<T>(T _, T v)
        {
            Refresh();
        }

        private void OnMouseDown()
        {
            OnCardClicked.Invoke();
        }
    }
}