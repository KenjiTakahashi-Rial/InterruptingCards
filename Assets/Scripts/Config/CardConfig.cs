using System.IO;

using UnityEngine;

using InterruptingCards.Models;
using System.Collections.Generic;

namespace InterruptingCards.Config
{
    public static class CardConfig
    {
        private const string PackFileExtension = "json";

        private static readonly string PackDirectory = Path.Combine("Assets", "Scripts", "Config", "Packs");

        private static readonly int IdBitCount = 32;
        private static readonly int SuitBitCount = IdBitCount / 2;
        private static readonly int RankBitCount = IdBitCount / 2;
        private static readonly int SuitBitMask = ~0 << (IdBitCount - SuitBitCount);
        private static readonly int RankBitMask = (1 << RankBitCount) - 1;

        public static int CardId(CardSuit suit, CardRank rank)
        {
            return ((int)suit & SuitBitMask) | ((int)rank & RankBitMask);
        }

        public static PackCard[] GetPack(CardPack pack)
        {
            var packName = pack.ToString();
            var packPath = Path.Combine(PackDirectory, packName);
            var cardPaths = Directory.GetFiles(packPath, "*." + PackFileExtension);
            var cards = new PackCard[cardPaths.Length];

            for (var i = 0; i < cardPaths.Length; i++)
            {
                var json = File.ReadAllText(cardPaths[i]);
                var card = JsonUtility.FromJson<PackCard>(json);
                cards[i] = card;
            }

            return cards;
        }
    }
}