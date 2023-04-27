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
        private readonly NetworkVariable<bool> _isFaceUp = new(true);
        private readonly NetworkVariable<PlayingCardSuit> _suit = new(PlayingCardSuit.Invalid);
        private readonly NetworkVariable<PlayingCardRank> _rank = new(PlayingCardRank.Invalid);

        [SerializeField] private TextMeshPro _cardText;

        private ICard<PlayingCardSuit, PlayingCardRank> _card = null;

        public event Action OnCardClicked;

        public bool IsFaceUp
        {
            get => _isFaceUp.Value;
            set
            {
                _isFaceUp.Value = value;
                Refresh();
            }
        }

        public ICard<PlayingCardSuit, PlayingCardRank> Card
        {
            get => _card;
            set
            {
                if (value == null)
                {
                    _suit.Value = PlayingCardSuit.Invalid;
                    _rank.Value = PlayingCardRank.Invalid;
                }
                else
                {
                    _suit.Value = value.Suit;
                    _rank.Value = value.Rank;
                }

                Refresh();
            }
        }

        public override void OnNetworkSpawn()
        {
            _suit.OnValueChanged -= Refresh;
            _rank.OnValueChanged -= Refresh;
            _isFaceUp.OnValueChanged -= Refresh;

            _suit.OnValueChanged += Refresh;
            _rank.OnValueChanged += Refresh;
            _isFaceUp.OnValueChanged += Refresh;
        }

        public override void OnNetworkDespawn()
        {
            _suit.OnValueChanged -= Refresh;
            _rank.OnValueChanged -= Refresh;
            _isFaceUp.OnValueChanged -= Refresh;
        }

        public void UnsubscribeAllOnCardClicked()
        {
            OnCardClicked = null;
        }

        private void Refresh()
        {
            if (_suit.Value == PlayingCardSuit.Invalid || _rank.Value == PlayingCardRank.Invalid)
            {
                _card = null;
                _cardText.gameObject.SetActive(false);
                return;
            }

            _cardText.gameObject.SetActive(true);

            if (_card == null)
            {
                _card = new PlayingCard(_suit.Value, _rank.Value);
            }
            else
            {
                _card.Suit = _suit.Value;
                _card.Rank = _rank.Value;
            }

            if (!IsFaceUp)
            {
                _cardText.enabled = false;
                return;
            }

            _cardText.SetText(_card.ToString()); // TODO: Change  
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