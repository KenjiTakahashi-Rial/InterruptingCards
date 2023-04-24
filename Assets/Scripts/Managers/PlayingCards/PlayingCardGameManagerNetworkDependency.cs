using Unity.Netcode;

using InterruptingCards.Factories;
using InterruptingCards.Models;

namespace InterruptingCards.Managers.GameManagers
{
    public class PlayingCardGameManagerNetworkDependency : NetworkBehaviour, IGameManagerNetworkDependency<PlayingCardSuit, PlayingCardRank>
    {
        private readonly PlayingCardGameManager _gameManager = new(PlayingCardPlayerFactory.Singleton, PlayingCardFactory.Singleton);

        internal static IGameManagerNetworkDependency<PlayingCardSuit, PlayingCardRank> Singleton { get; private set; }

        public override void OnNetworkSpawn()
        {
            Singleton = this;
            _gameManager.OnNetworkSpawn();
        }

        public override void OnNetworkDespawn()
        {
            _gameManager.OnNetworkDespawn();
            Singleton = null;
        }

        [ServerRpc]
        void IGameManagerNetworkDependency<PlayingCardSuit, PlayingCardRank>.AddPlayerServerRpc(ulong clientId)
        {
            _gameManager.AddPlayer(clientId);
        }

        [ServerRpc]
        void IGameManagerNetworkDependency<PlayingCardSuit, PlayingCardRank>.RemovePlayerServerRpc(ulong clientId)
        {
            _gameManager.RemovePlayer(clientId);
        }

        [ServerRpc]
        void IGameManagerNetworkDependency<PlayingCardSuit, PlayingCardRank>.GetSelfServerRpc(ServerRpcParams serverRpcParams)
        {
            _gameManager.GetSelf(serverRpcParams);
        }

        [ClientRpc]
        void IGameManagerNetworkDependency<PlayingCardSuit, PlayingCardRank>.AssignSelfClientRpc(ClientRpcParams clientRpcParams)
        {
            _gameManager.AssignSelf(clientRpcParams);
        }

        [ServerRpc]
        void IGameManagerNetworkDependency<PlayingCardSuit, PlayingCardRank>.DealHandsServerRpc()
        {
            _gameManager.DealHands();
        }

        [ServerRpc]
        void IGameManagerNetworkDependency<PlayingCardSuit, PlayingCardRank>.DrawCardServerRpc(ServerRpcParams serverRpcParams)
        {
            _gameManager.DrawCard(serverRpcParams);
        }

        [ServerRpc]
        void IGameManagerNetworkDependency<PlayingCardSuit, PlayingCardRank>.PlayCardServerRpc(PlayingCardSuit suit, PlayingCardRank rank, ServerRpcParams serverRpcParams)
        {
            _gameManager.PlayCard(suit, rank, serverRpcParams);
        }
    }
}
