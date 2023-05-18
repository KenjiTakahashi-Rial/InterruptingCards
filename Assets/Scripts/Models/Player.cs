using Unity.Netcode;

using InterruptingCards.Managers;

namespace InterruptingCards.Models
{
    public class Player : INetworkSerializable
    {
        private ulong _id;
        private string _name;

        // The empty constructor is necessary for network seriaization
        // TODO: Try making not public
        public Player() { }

        internal Player(ulong id, string name)
        {
            _id = id;
            _name = name;
        }

        public ulong Id => _id;

        public string Name => _name;

        public HandManager Hand { get; set; }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _id);
            serializer.SerializeValue(ref _name);
        }
    }
}