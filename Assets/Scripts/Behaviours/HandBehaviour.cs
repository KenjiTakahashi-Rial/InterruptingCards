using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Netcode;
using UnityEditor;
using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Managers;
using System.Collections;

namespace InterruptingCards.Behaviours
{
    [ExecuteInEditMode]
    public class HandBehaviour : NetworkBehaviour, IEnumerable<CardBehaviour>
    {
        private readonly CardConfig _cardConfig = CardConfig.Singleton;

#pragma warning disable RCS1169 // Make field read-only.
        [Header("Config")]
        [SerializeField] private RectTransform _handRectTransform;
        [SerializeField] private CardBehaviour[] _cardSlots;
        [SerializeField] private RectTransform[] _cardRectTransforms;
        [SerializeField] private Vector2 _spacing;
        [SerializeField] private int _maxColumns;

        [Header("Editor")]
        [SerializeField] private int _previewCount;
#pragma warning restore RCS1169 // Make field read-only.

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
            if (_cardSlots.Length != _cardRectTransforms.Length)
            {
                Log.Error(
                    $"Card slot count ({_cardSlots.Length}) does not match card RectTransform count " +
                    $"({_cardRectTransforms.Length})"
                );
            }

            for (var i = 0; i < _cardSlots.Length; i++)
            {
                var card = _cardSlots[i];
                card.OnClicked += () => OnCardClicked?.Invoke(card.CardId);
            }
        }

        public void Update()
        {
            // TODO: Consider doing in response to changes instead of every frame
            UpdateLayout();
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
                card.IsHidden = val;
            }
        }

        public void Add(int cardId)
        {
            if (Count == _cardSlots.Length)
            {
                throw new TooManyCardsException("Cannot add more cards than card slots");
            }

            Log.Info($"Adding {_cardConfig.GetName(cardId)} to hand");
            _cardSlots[Count].CardId = cardId;
        }

        public void Insert(int index, int cardId)
        {
            if (index < 0 || index > Count)
            {
                throw new CardIndexOutOfRangeException($"Cannot insert into index {index} for count {Count}");
            }

            if (Count == _cardSlots.Length)
            {
                throw new TooManyCardsException("Cannot add more cards than card slots");
            }

            Log.Info($"Inserting {_cardConfig.GetName(cardId)} to hand in position {index}");
            for (int i = Count; i > index; i--)
            {
                _cardSlots[i].CardId = _cardSlots[i - 1].CardId;
            }
            _cardSlots[index].CardId = cardId;
        }

        public int Remove(int cardId)
        {
            Log.Info($"Removing {_cardConfig.GetName(cardId)} from hand");
            var index = Array.FindIndex(_cardSlots, s => s.CardId == cardId);
            for (int i = index; i < Count; i++)
            {
                _cardSlots[i].CardId = i == _cardSlots.Length - 1 ? CardConfig.InvalidId : _cardSlots[i + 1].CardId;
            }

            return cardId;
        }

        public int IndexOf(int cardId)
        {
            return Array.FindIndex(_cardSlots, s => s.CardId == cardId);
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

        private bool ShouldIncludeInLayout(CardBehaviour card)
        {
            return
#if UNITY_EDITOR
                !EditorApplication.isPlaying ||
#endif
                !card.IsHidden && card.CardId != CardConfig.InvalidId;
        }

        private void UpdateLayout()
        {
            var cardSize = _cardRectTransforms[0].sizeDelta;
            var activeCount =
#if UNITY_EDITOR
                Math.Min(
                    EditorApplication.isPlaying ? int.MaxValue : _previewCount,
#endif
                    _cardSlots.Count(ShouldIncludeInLayout)
#if UNITY_EDITOR
                )
#endif
            ;

            var rows = Mathf.CeilToInt((float)activeCount / _maxColumns);
            var columns = Math.Min(_maxColumns, activeCount);

            var totalWidth = (columns * cardSize.x) + ((columns - 1) * _spacing.x);
            var totalHeight = (rows * cardSize.y) + ((rows - 1) * _spacing.y);
            _handRectTransform.sizeDelta = new Vector2(totalWidth, totalHeight);

            var startPosition = new Vector2((-totalWidth / 2) + (cardSize.x / 2), (totalHeight / 2) - (cardSize.y / 2));

#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                for (var i = 0; i < _cardSlots.Length; i++)
                {
                    _cardSlots[i].IsHidden = i >= activeCount;
                }
            }
#endif

            var cardIndex = 0;
            for (var row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (cardIndex >= _cardSlots.Length)
                    {
                        return;
                    }

                    if (ShouldIncludeInLayout(_cardSlots[cardIndex]))
                    {
                        Vector2 anchoredPosition = startPosition + new Vector2(col * (cardSize.x + _spacing.x), -row * (cardSize.y + _spacing.y));
                        RectTransform rectTransform = _cardRectTransforms[cardIndex];

                        rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                        rectTransform.pivot = new Vector2(0.5f, 0.5f);
                        rectTransform.anchoredPosition = anchoredPosition;

                        cardIndex++;
                    }
                }
            }
        }
    }
}
