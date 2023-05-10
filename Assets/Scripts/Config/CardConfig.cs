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

        public static int CardId(ICard card)
        {
            return ((int)card.Suit & SuitBitMask) | ((int)card.Rank & RankBitMask);
        }

        public static ImmutableDictionary<C, int> GetCardCounts<C>(CardPack pack) where C : ICard
        {
            var packName = pack.ToString();
            var packPath = Path.Combine(PackDirectory, packName);
            var cardPaths = Directory.GetFiles(packPath, "*." + PackFileExtension);
            var counts = new Dictionary<C, int>(cardPaths.Length);

            for (var i = 0; i < cardPaths.Length; i++)
            {
                var json = File.ReadAllText(cardPaths[i]);
                var metadata = JsonUtility.FromJson<MetadataCard>(json);
                var card = JsonUtility.FromJson<C>(json);
                counts[card] = metadata.Count;
            }

            return new ImmutableDictionary<C, int>(counts);
        }
    }
}