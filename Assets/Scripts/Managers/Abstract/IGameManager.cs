using Unity.Netcode;

using InterruptingCards.Models;

namespace InterruptingCards.Managers.GameManagers
{
    public interface IGameManager
    {
        static IGameManager Singleton { get; }

        void OnNetworkSpawn();

        void OnNetworkDespawn();

        void AddPlayer(ulong clientId);

        void RemovePlayer(ulong clientId);

        void StartGame();

        void GetSelf(ServerRpcParams serverRpcParams);

        void AssignSelf(ClientRpcParams clientRpcParams);

        void DealHands();

        void DrawCard(ServerRpcParams serverRpcParams);

        void PlayCard(SuitEnum suit, RankEnum rank, ServerRpcParams serverRpcParams);
    }
}
