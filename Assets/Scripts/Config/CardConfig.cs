using System.Collections.Generic;
using System.IO;

using UnityEngine;

using InterruptingCards.Models;

namespace InterruptingCards.Config
{
    public static class CardConfig
    {
        private const string PackFileExtension = "json";

        public static readonly int InvalidId = GetCardId(CardSuit.Invalid, CardRank.Invalid);

        private static readonly string PackDirectory = Path.Combine("Assets", "Scripts", "Config", "Packs");

        private static readonly int IdBitCount = 32;
        private static readonly int SuitBitCount = IdBitCount / 2;
        private static readonly int RankBitCount = IdBitCount / 2;
        private static readonly int SuitBitMask = ~0 << (IdBitCount - SuitBitCount);
        private static readonly int RankBitMask = (1 << RankBitCount) - 1;

        private static readonly Dictionary<int, MetadataCard> _cards = new();

        public static int GetCardId(CardSuit suit, CardRank rank)
        {
            return ((int)suit << RankBitCount & SuitBitMask) | ((int)rank & RankBitMask);
        }

        public static MetadataCard GetMetadataCard(int id)
        {
            return _cards[id];
        }

        public static MetadataCard[] GetPackMetadata(CardPack pack)
        {
            var packName = pack.ToString();
            var packPath = Path.Combine(PackDirectory, packName);
            var cardPaths = Directory.GetFiles(packPath, "*." + PackFileExtension);
            var cards = new MetadataCard[cardPaths.Length];

            for (var i = 0; i < cardPaths.Length; i++)
            {
                var json = File.ReadAllText(cardPaths[i]);
                var card = JsonUtility.FromJson<MetadataCard>(json);

                _cards[card.Id] = card;
                cards[i] = card;
            }

            return cards;
        }
    }
}