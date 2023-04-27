using Unity.Netcode;

using InterruptingCards.Models;

namespace InterruptingCards.Managers.GameManagers
{
    public class PlayingCardGameManagerNetworkDependency : NetworkBehaviour, IGameManagerNetworkDependency<PlayingCardSuit, PlayingCardRank>
    {
        public static IGameManagerNetworkDependency<PlayingCardSuit, PlayingCardRank> Singleton { get; private set; }

        private IGameManager<PlayingCardSuit, PlayingCardRank> GameManager { get => PlayingCardGameManager.Singleton; }

        public bool IsSelfHost => IsHost;

        public override void OnNetworkSpawn()
        {
            Singleton = this;
            GameManager.OnNetworkSpawn();
        }

        public override void OnNetworkDespawn()
        {
            GameManager.OnNetworkDespawn();
            Singleton = null;
        }

        [ServerRpc]
        public void AddPlayerServerRpc(ulong clientId)
        {
            GameManager.AddPlayer(clientId);
        }

        [ServerRpc]
        public void RemovePlayerServerRpc(ulong clientId)
        {
            GameManager.RemovePlayer(clientId);
        }

        [ServerRpc(RequireOwnership = false)]
        public void GetSelfServerRpc(ServerRpcParams serverRpcParams = default)
        {
            GameManager.GetSelf(serverRpcParams);
        }

        [ClientRpc]
        public void AssignSelfClientRpc(ClientRpcParams clientRpcParams)
        {
            GameManager.AssignSelf(clientRpcParams);
        }

        [ServerRpc]
        public void DealHandsServerRpc()
        {
            GameManager.DealHands();
        }

        [ServerRpc]
        public void DrawCardServerRpc(ServerRpcParams serverRpcParams = default)
        {
            GameManager.DrawCard(serverRpcParams);
        }

        [ServerRpc]
        public void PlayCardServerRpc(PlayingCardSuit suit, PlayingCardRank rank, ServerRpcParams serverRpcParams = default)
        {
            GameManager.PlayCard(suit, rank, serverRpcParams);
        }
    }
}
