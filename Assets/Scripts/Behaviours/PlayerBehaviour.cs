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
                    var truncated = Utilities.Functions.Truncate(value, NameByteLimit);
                    Log.Warn(
                        $"String \"{value}\" ({byteCount}B) exceeds player name byte limit ({NameByteLimit}B). " +
                        $"Truncating to {truncated}"
                    );
                }
                _name.Value = value;
            }
        }

        public HandBehaviour Hand { get; set; }

        public Dictionary<int, CardBehaviour> ActiveItems { get; set; }

        public uint Money { get => _money.Value; set => _money.Value = value; }

        public uint LootPlays { get => _lootPlays.Value; set => _lootPlays.Value = value; }
    }
}