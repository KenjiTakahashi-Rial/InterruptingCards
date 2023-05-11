using System;

using UnityEngine;

using InterruptingCards.Config;

namespace InterruptingCards.Models
{
    [Serializable]
    public class MetadataCard
    {
        /******************************************************************************************\
         * JSON keys                                                                              *
        \******************************************************************************************/

        [SerializeField] private string _suitName;

        [SerializeField] private string _rankName;

        [SerializeField] private string _name;

        [SerializeField] private int _count;

        [SerializeField] private string _activeEffectName;

        /******************************************************************************************\
         * Lazy backing fields                                                                    *
        \******************************************************************************************/

        private Lazy<int> _id;

        private Lazy<CardSuit> _suit;

        private Lazy<CardRank> _rank;

        private Lazy<CardActiveEffect> _activeEffect;

        /******************************************************************************************\
         * Public getters                                                                         *
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
         * Helper methods                                                                         *
        \******************************************************************************************/

        private Func<E> EnumValueFactory<E>(string name)
        {
            return () => (E)Enum.Parse(typeof(E), name);
        }
    }
}