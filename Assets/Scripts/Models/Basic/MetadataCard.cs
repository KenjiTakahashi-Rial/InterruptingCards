using System;

using UnityEngine;

using InterruptingCards.Config;

namespace InterruptingCards.Models
{
    [Serializable]
    public class MetadataCard
    {
        [SerializeField] private string _suitName;

        [SerializeField] private string _rankName;

        [SerializeField] private string _name;

        [SerializeField] private int _count;

        private Lazy<int> _id;

        private Lazy<CardSuit> _suit;

        private Lazy<CardRank> _rank;

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
                _suit ??= new(() => (CardSuit)Enum.Parse(typeof(CardSuit), _suitName));
                return _suit.Value;
            }
        }

        public CardRank Rank
        {
            get
            {
                _rank ??= new(() => (CardRank)Enum.Parse(typeof(CardRank), _rankName));
                return _rank.Value;
            }
        }

        public string Name => _name;

        public int Count => _count;
    }
}