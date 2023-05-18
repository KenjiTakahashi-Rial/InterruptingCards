using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;

using InterruptingCards.Models;

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

        private readonly string _packDirectory = Path.Combine("Assets", "Scripts", "Config", "Packs");

        private readonly Dictionary<int, Card> _cards = new();

        private CardConfig() { }

        public Card this[int cardId] => _cards[cardId];

        public static int GetCardId(CardSuit suit, CardRank rank)
        {
            return (((int)suit << RankBitCount) & SuitBitMask) | ((int)rank & RankBitMask);
        }

        public string GetCardString(int id)
        {
            if (_cards.ContainsKey(id))
            {
                return _cards[id].ToString();
            }

            var suit = (CardSuit)((id & SuitBitMask) >> RankBitCount);
            var rank = (CardRank)(id & RankBitMask);
            return $"{rank} | {suit}";
        }

        public void Load(CardPack cardPack)
        {
            Debug.Log($"CardConfig loading {cardPack}");

            _cards.Clear();

            var packName = cardPack.ToString();
            var packPath = Path.Combine(_packDirectory, packName);
            var cardPaths = Directory.GetFiles(packPath, "*." + PackFileExtension);

            for (var i = 0; i < cardPaths.Length; i++)
            {
                var json = File.ReadAllText(cardPaths[i]);
                var card = JsonUtility.FromJson<ParserCard>(json);
                var id = card.Id;

                if (_cards.ContainsKey(id))
                {
                    Debug.LogWarning($"Cache already contains {id}. Overwriting {_cards[id]} with {card.Name}");
                }

                _cards[card.Id] = new Card(card);
            }
        }

        public int[] GenerateDeck()
        {
            var values = _cards.Values;
            var expanded = values.SelectMany(c => Enumerable.Repeat(c.Id, c.Count));
            var asArr = expanded.ToArray();
            return asArr;
        }
    }
}