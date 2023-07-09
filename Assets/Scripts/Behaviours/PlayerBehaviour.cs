using System.Collections.Generic;
using System.Text;

using Unity.Collections;
using Unity.Netcode;

using InterruptingCards.Managers;
using InterruptingCards.Utilities;

namespace InterruptingCards.Behaviours
{
    public class PlayerBehaviour : NetworkBehaviour
    {
        private const int NameByteLimit = 32;

        private readonly NetworkVariable<ulong> _id = new();
        private readonly NetworkVariable<FixedString32Bytes> _name = new();
        private readonly NetworkVariable<uint> _money = new();
        private readonly NetworkVariable<uint> _lootPlays = new();
        private readonly NetworkVariable<uint> _purchases = new();

        public ulong Id { get => _id.Value; set => _id.Value = value; }

        private LogManager Log => LogManager.Singleton;

        public string Name
        {
            get => _name.Value.ToString();
            set
            {
                var byteCount = Encoding.UTF8.GetByteCount(value);
                if (byteCount > NameByteLimit)
                {
                    var truncated = Functions.Truncate(value, NameByteLimit);
                    Log.Warn(
                        $"String \"{value}\" ({byteCount}B) exceeds player name byte limit ({NameByteLimit}B). " +
                        $"Truncating to {truncated}"
                    );
                }
                _name.Value = value;
            }
        }

        public CardBehaviour CharacterCard { get; set; }

        //public HandBehaviour Items { get; set; }

        public HandBehaviour Hand { get; set; }

        public uint Money { get => _money.Value; set => _money.Value = value; }

        public uint LootPlays { get => _lootPlays.Value; set => _lootPlays.Value = value; }

        public uint Purchases { get => _purchases.Value; set => _purchases.Value = value; }

        public uint PurchaseCost => 10; // TODO

        public IReadOnlyList<CardBehaviour> ActivatedCards
        {
            get
            {
                var activatedCards = new List<CardBehaviour>();

                if (!CharacterCard.IsDeactivated)
                {
                    activatedCards.Add(CharacterCard);
                }

                //foreach (var card in Items)
                //{
                //    if (!card.IsDeactivated)
                //    {
                //        activatedCards.Add(card);
                //    }
                //}

                return activatedCards;
            }
        }
    }
}