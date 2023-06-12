using Unity.Netcode;

using InterruptingCards.Managers;

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
    }
}