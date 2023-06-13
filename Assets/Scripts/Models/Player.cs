using InterruptingCards.Behaviours;

namespace InterruptingCards.Models
{
    public class Player
    {
        internal Player(ulong id, string name)
        {
            Id = id;
            Name = name;
        }

        public ulong Id { get; }

        public string Name { get; }

        public HandBehaviour Hand { get; set; }

        // TODO: Make money and loot plays into NetworkVariables and this class into NetworkBehaviour
        public uint Money { get; set; }

        public uint LootPlays { get; set; }
    }
}