using Unity.Netcode;

namespace InterruptingCards.Models
{
    public interface IPlayer : INetworkSerializable
    {
        ulong Id { get; }

        string Name { get; }

        IHand Hand { get; set; }
    }
}