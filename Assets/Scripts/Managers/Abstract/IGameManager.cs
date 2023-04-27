using System;

using Unity.Netcode;

namespace InterruptingCards.Managers.GameManagers
{
    public interface IGameManager<S, R> where S : Enum where R : Enum
    {
        static IGameManager<S, R> Singleton { get; }

        void OnNetworkSpawn();

        void OnNetworkDespawn();

        void AddPlayer(ulong clientId);

        void RemovePlayer(ulong clientId);

        void StartGame();

        void GetSelf(ServerRpcParams serverRpcParams);

        void AssignSelf(ClientRpcParams clientRpcParams);

        void DealHands();

        void DrawCard(ServerRpcParams serverRpcParams);

        void PlayCard(S suit, R rank, ServerRpcParams serverRpcParams);
    }
}
