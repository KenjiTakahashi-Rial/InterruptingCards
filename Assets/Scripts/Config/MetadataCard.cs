using System;

using UnityEngine;

using InterruptingCards.Config;

namespace InterruptingCards.Models
{
    [Serializable]
    public class MetadataCard
    {
        /******************************************************************************************\
         * JSON Keys                                                                              *
        \******************************************************************************************/

        [SerializeField] private string _suitName;

        [SerializeField] private string _rankName;

        [SerializeField] private string _name;

        [SerializeField] private int _count;

        [SerializeField] private string _activeEffectName;

        /******************************************************************************************\
         * Lazy Backing Fields                                                                    *
        \******************************************************************************************/

        private Lazy<int> _id;

        private Lazy<CardSuit> _suit;

        private Lazy<CardRank> _rank;

        private Lazy<CardActiveEffect> _activeEffect;

        /******************************************************************************************\
         * Public Getters                                                                         *
        \******************************************************************************************/

        public int Id
        {
            get
            {
                _id ??= new(() => CardConfig.GetCardId(Suit, Rank));
                return _id.Value;
            }
        }

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

        public int Count => _count;

        public CardActiveEffect ActiveEffect
        {
            get
            {
                _activeEffect ??= new(EnumValueFactory<CardActiveEffect>(_activeEffectName));
                return _activeEffect.Value;
            }
        }

        /******************************************************************************************\
         * Helper Methods                                                                         *
        \******************************************************************************************/

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