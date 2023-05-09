using System;
using System.Collections.Generic;
using System.Linq;

using InterruptingCards.Models;

namespace InterruptingCards.Config
{
    public class PlayingCardConfig
    {
        private const string DeckName = "PlayingCards";

        private static readonly int IdBitCount = 32;
        private static readonly int SuitBitCount = IdBitCount / 2;
        private static readonly int RankBitCount = IdBitCount / 2;
        private static readonly int SuitBitMask = ~0 << (IdBitCount - SuitBitCount);
        private static readonly int RankBitMask = (1 << RankBitCount) - 1;

        private static readonly Dictionary<> Cards;

        public PlayingCardConfig()
        {
            
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