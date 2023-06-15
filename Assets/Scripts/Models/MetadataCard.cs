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

        [SerializeField] private string _lootAbilityName;

        [SerializeField] private string _activatedAbilityName;

        // Lazy Backing Fields

        private Lazy<CardSuit> _suit;

        private Lazy<CardRank> _rank;

        private Lazy<CardAbility> _lootAbility;

        private Lazy<CardAbility> _activatedAbility;

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

        public CardAbility LootAbility
        {
            get
            {
                _lootAbility ??= new(EnumValueFactory<CardAbility>(_lootAbilityName));
                return _lootAbility.Value;
            }
        }

        public CardAbility ActivatedAbility
        {
            get
            {
                _activatedAbility ??= new(EnumValueFactory<CardAbility>(_activatedAbilityName));
                return _activatedAbility.Value;
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