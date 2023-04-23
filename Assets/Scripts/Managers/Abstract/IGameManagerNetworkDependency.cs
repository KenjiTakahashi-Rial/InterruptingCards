using System;

using Unity.Netcode;

namespace InterruptingCards.Managers.GameManagers
{
    // AbstractGameManager is generic, so it cannot inherit from NetworkBehaviour
    // Couple the concrete game manager with an implementation of this for networking capabilities
    public interface IGameManagerNetworkDependency<S, R> where S : Enum where R : Enum
    {
        internal static IGameManagerNetworkDependency<S, R> Singleton { get; }

        [ServerRpc]
        internal void AddPlayerServerRpc(ulong clientId);
        [ServerRpc]
        internal void RemovePlayerServerRpc(ulong clientId);
        [ServerRpc]
        internal void GetSelfServerRpc(ServerRpcParams serverRpcParams = default);
        [ClientRpc]
        internal void AssignSelfClientRpc(ClientRpcParams clientRpcParams);
        [ServerRpc]
        internal void DealHandsServerRpc();
        [ServerRpc]
        internal void DrawCardServerRpc(ServerRpcParams serverRpcParams = default);
        [ServerRpc]
        internal void PlayCardServerRpc(S suit, R rank, ServerRpcParams serverRpcParams = default);
    }
}
