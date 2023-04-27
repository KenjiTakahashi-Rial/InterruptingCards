using Unity.Netcode;

using InterruptingCards.Models;

namespace InterruptingCards.Managers.GameManagers
{
    // AbstractGameManager is generic, so it cannot inherit perform ServerRpc
    // Couple the concrete game manager with an implementation of this dependency to allow ServerRpc calls
    public interface IGameManagerNetworkDependency
    {
        static IGameManagerNetworkDependency Singleton { get; }

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
        void PlayCardServerRpc(SuitEnum suit, RankEnum rank, ServerRpcParams serverRpcParams = default);
    }
}
