using Unity.Netcode;

using InterruptingCards.Managers;

namespace InterruptingCards.Models
{
    public class Player
    {
        private ulong _id;
        private string _name;

        internal Player(ulong id, string name)
        {
            _id = id;
            _name = name;
        }

        public ulong Id => _id;

        public string Name => _name;

        public HandManager Hand { get; set; }
    }
}