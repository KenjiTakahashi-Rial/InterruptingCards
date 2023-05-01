using Unity.Netcode;

namespace InterruptingCards.Models
{
    public abstract class AbstractPlayer : IPlayer
    {
        protected ulong _id;
        protected string _name;
        protected IHand _hand;

        protected AbstractPlayer() { }

        protected AbstractPlayer(ulong id, string name, IHand hand = null)
        {
            _id = id;
            _name = name;
            _hand = hand;
        }

        public virtual ulong Id => _id;

        public virtual string Name => _name;

// The inner field is necessary for network serialization
#pragma warning disable S2292 // Trivial properties should be auto-implemented
        public virtual IHand Hand { get => _hand; set => _hand = value; }
#pragma warning restore S2292 // Trivial properties should be auto-implemented

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _id);
            serializer.SerializeValue(ref _name);
            // Do not serialize hand since the cards are all NetworkVariables so they are sync'd
        }
    }
}