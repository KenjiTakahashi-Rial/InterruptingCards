using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;

using InterruptingCards.Managers;
using InterruptingCards.Models;
using InterruptingCards.Utilities;

namespace InterruptingCards.Config
{
    public class CardConfig
    {
        public const int InvalidId = 0;

        private const string InvalidName = "Invalid";
        private const string PackFileExtension = "json";

        public static readonly CardConfig Singleton = new();

        private readonly string _packDirectory = Path.Combine("Assets", "Scripts", "Config", "Packs");

        private readonly Dictionary<int, Card> _cards = new();

        private CardConfig() { }

        private LogManager Log => LogManager.Singleton;

        public Card this[int cardId] => _cards[cardId];

        public string GetName(int id)
        {
            return id == InvalidId ? InvalidName : _cards[id].ToString();
        }

        public void Load(CardPack cardPack)
        {
            Log.Info($"CardConfig loading {cardPack}");

            _cards.Clear();

            var packName = cardPack.ToString();
            var packPath = Path.Combine(_packDirectory, packName);
            var searchPattern = "*." + PackFileExtension;

            var id = 1;

            void LoadCard(string fileName)
            {
                var json = File.ReadAllText(fileName);
                var card = JsonUtility.FromJson<MetadataCard>(json);

                for (var j = 0; j < card.Count; j++)
                {
                    _cards[id] = new Card(id++, card);
                }
            }

            Helpers.ForEachFile(packPath, LoadCard, recursive: true, fileSearchPattern: searchPattern);
        }

        public List<int> GenerateIdDeck(Func<Card, bool> predicate = null)
        {
            predicate ??= c => true;

            return _cards
                .Where(pair => predicate(pair.Value))
                .Select(pair => pair.Key)
                .ToList();
        }
    }
}