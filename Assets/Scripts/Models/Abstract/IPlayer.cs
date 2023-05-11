using Unity.Netcode;

namespace InterruptingCards.Models
{
    public interface IPlayer<C, H> : INetworkSerializable where C : ICard where H : IHand<C>
    {
        ulong Id { get; }

        string Name { get; }

        H Hand { get; set; }
    }
}