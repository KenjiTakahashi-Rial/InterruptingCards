using Unity.Netcode;

namespace InterruptingCards.Models
{
    public class BasicPlayer : IPlayer<BasicCard, IHand<BasicCard>>
    {
        protected ulong _id;
        protected string _name;
        protected IHand<BasicCard> _hand;

        // The empty constructor is necessary for network seriaization
        public BasicPlayer() { }

        // Do not call directly; use a factory
        public BasicPlayer(ulong id, string name)
        {
            _id = id;
            _name = name;
        }

        public virtual ulong Id => _id;

        public virtual string Name => _name;

// The inner field is necessary for network serialization
#pragma warning disable S2292 // Trivial properties should be auto-implemented
        public virtual IHand<BasicCard> Hand { get => _hand; set => _hand = value; }
#pragma warning restore S2292 // Trivial properties should be auto-implemented

        public virtual void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _id);
            serializer.SerializeValue(ref _name);
            // Do not serialize hand since the cards are sync'd by NetworkVariable
        }
    }
}