using System.Collections.Generic;
using System.IO;

using UnityEngine;

using InterruptingCards.Models;
using InterruptingCards.Utilities;

namespace InterruptingCards.Config
{
    public class CardConfig
    {
        private const string PackFileExtension = "json";

        public static readonly CardConfig Singleton = new();
        public static readonly int InvalidId = GetCardId(CardSuit.Invalid, CardRank.Invalid);

        private static readonly int IdBitCount = 32;
        private static readonly int SuitBitCount = IdBitCount / 2;
        private static readonly int RankBitCount = IdBitCount / 2;
        private static readonly int SuitBitMask = ~0 << (IdBitCount - SuitBitCount);
        private static readonly int RankBitMask = (1 << RankBitCount) - 1;

        private readonly string PackDirectory = Path.Combine("Assets", "Scripts", "Config", "Packs");

        private readonly Dictionary<int, MetadataCard> _cards = new();
        private readonly Dictionary<CardPack, ImmutableList<MetadataCard>> _packs = new();

        private CardConfig() { }

        public static int GetCardId(CardSuit suit, CardRank rank)
        {
            return ((int)suit << RankBitCount & SuitBitMask) | ((int)rank & RankBitMask);
        }

        public MetadataCard GetMetadataCard(int id)
        {
            if (!_cards.ContainsKey(id))
            {
                Debug.LogWarning($"Card {id} not found. Is the card pack loaded?");
            }

            return _cards[id];
        }

        public ImmutableList<MetadataCard> GetCardPack(CardPack cardPack)
        {
            if (!_packs.ContainsKey(cardPack))
            {
                Load(cardPack);
            }
            
            return _packs[cardPack];
        }

        private void Load(CardPack cardPack)
        {
            var packName = cardPack.ToString();
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

            _packs[cardPack] = new ImmutableList<MetadataCard>(cards);
        }
    }
}