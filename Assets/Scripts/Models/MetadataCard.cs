using System;

using UnityEngine;

namespace InterruptingCards.Models
{
    [Serializable]
    public class MetadataCard
    {
        // JSON Keys

        [SerializeField] private string _suitName;

        [SerializeField] private string _rankName;

        [SerializeField] private string _name;

        [SerializeField] private uint _count;

        [SerializeField] private int _value;

        [SerializeField] private string _playedEffectName;

        [SerializeField] private string _activeEffectName;

        // Lazy Backing Fields

        private Lazy<CardSuit> _suit;

        private Lazy<CardRank> _rank;

        private Lazy<CardPlayedEffect> _playedEffect;

        private Lazy<CardActiveEffect> _activeEffect;

        // Public Getters

        public CardSuit Suit
        {
            get
            {
                _suit ??= new(EnumValueFactory<CardSuit>(_suitName));
                return _suit.Value;
            }
        }

        public CardRank Rank
        {
            get
            {
                _rank ??= new(EnumValueFactory<CardRank>(_rankName));
                return _rank.Value;
            }
        }

        public string Name => _name;

        public uint Count => _count;

        public int Value => _value;

        public CardPlayedEffect PlayedEffect
        {
            get
            {
                _playedEffect ??= new(EnumValueFactory<CardPlayedEffect>(_playedEffectName));
                return _playedEffect.Value;
            }
        }

        public CardActiveEffect ActiveEffect
        {
            get
            {
                _activeEffect ??= new(EnumValueFactory<CardActiveEffect>(_activeEffectName));
                return _activeEffect.Value;
            }
        }

        // Helper Methods

        private Func<E> EnumValueFactory<E>(string name) where E : struct
        {
            return () => 
            {
                Enum.TryParse(name, out E outValue);
                return outValue;
            };
        }
    }
}