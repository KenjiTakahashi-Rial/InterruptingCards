using System;
using System.Collections.Generic;
using System.Linq;

using InterruptingCards.Models;

namespace InterruptingCards.Config
{
    public class PlayingCardConfig
    {
        private static readonly int IdBitCount = 32;
        private static readonly int SuitBitCount = IdBitCount / 2;
        private static readonly int RankBitCount = IdBitCount / 2;
        private static readonly int SuitBitMask = ~0 << (IdBitCount - SuitBitCount);
        private static readonly int RankBitMask = (1 << RankBitCount) - 1;

        private static readonly Dictionary<(CardSuit, CardRank), int> CardCounts = new();
        private static readonly 

        public PlayingCardConfig()
        {
            var suits = Enum.GetValues(typeof(CardSuit))
                .Cast<CardSuit>()
                .Where(e => e >= CardSuit.Clubs && e <= CardSuit.Spades)
                .ToList();
            var ranks = Enum.GetValues(typeof(CardRank))
                .Cast<CardRank>()
                .Where(e => e >= CardRank.Ace && e <= CardRank.King)
                .ToList();

            foreach (var suit in suits)
            {
                foreach (var rank in ranks)
                {
                    CardCounts[(suit, rank)] = 1;
                }
            }
        }

        public static int CardId(CardSuit suit, CardRank rank)
        {
            return ((int)suit & SuitBitMask) | ((int)rank & RankBitMask);
        }

        public static ICard CardById(int id)
        {

        }
    }
}