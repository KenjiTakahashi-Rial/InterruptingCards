using System;

using Unity.Netcode;

namespace InterruptingCards.Managers.GameManagers
{
    // AbstractGameManager is generic, so it cannot inherit perform ServerRpc
    // Couple the concrete game manager with an implementation of this dependency to allow ServerRpc calls
    public interface IGameManagerNetworkDependency<S, R> where S : Enum where R : Enum
    {
        static IGameManagerNetworkDependency<S, R> Singleton { get; }

        bool IsSelfHost { get; }

        [ServerRpc]
        void AddPlayerServerRpc(ulong clientId);

        [ServerRpc]
        void RemovePlayerServerRpc(ulong clientId);

        [ServerRpc(RequireOwnership = false)]
        void GetSelfServerRpc(ServerRpcParams serverRpcParams = default);

        [ClientRpc]
        void AssignSelfClientRpc(ClientRpcParams clientRpcParams);

        [ServerRpc]
        void DealHandsServerRpc();

        [ServerRpc]
        void DrawCardServerRpc(ServerRpcParams serverRpcParams = default);

        [ServerRpc]
        void PlayCardServerRpc(S suit, R rank, ServerRpcParams serverRpcParams = default);
    }
}
