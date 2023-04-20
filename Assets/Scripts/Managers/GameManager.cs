using System.Collections.Generic;

using Unity.Netcode;

using InterruptingCards.Models.Abstract;
using InterruptingCards.Models.PlayingCards;

namespace InterruptingCards.Managers
{
    public class GameManager : NetworkBehaviour, IGame
    {
        private readonly List<Player> _players = new();
        private Deck<Suit, Rank> _deck;

        public override void OnNetworkSpawn()
        {
            NetworkManagerDecorator.Singleton.OnClientConnectedCallback -= AddPlayer;
            NetworkManagerDecorator.Singleton.OnClientConnectedCallback += AddPlayer;

            NetworkManagerDecorator.Singleton.OnClientDisconnectCallback -= RemovePlayer;
            NetworkManagerDecorator.Singleton.OnClientDisconnectCallback += RemovePlayer;
        }
        
        public void EndTurn()
        {
            throw new System.NotImplementedException();
        }

        public void Play()
        {
            throw new System.NotImplementedException();
        }

        private void AddPlayer(ulong clientId)
        {
            _players.Add(new Player(clientId.ToString(), new Hand()));
        }

        private void RemovePlayer(ulong clientId)
        {
            _players.RemoveAll(p => p.Name == clientId.ToString());
        }
    }
}
