using System;

using Unity.Netcode;

using InterruptingCards.Factories;
using InterruptingCards.Serialization;

namespace InterruptingCards.Managers.GameManagers
{
    public class PlayingCardGameManagerNetworkDependency : NetworkBehaviour, IGameManagerNetworkDependency
    {
        private readonly PlayingCardGameManager _gameManager = new(PlayingCardPlayerFactory.Singleton);

        internal static IGameManagerNetworkDependency Singleton { get; private set; }

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

        void IGameManagerNetworkDependency.DoOperation(Operation operation, object[] args, bool clientRpc, bool requireOwnership)
        {
            if (clientRpc)
            {
                DoOperationClient(operation, args);
            }
            else if (requireOwnership)
            {
                DoOperationServerRequireOwnership(operation, args);
            }
            else
            {
                DoOperationServerNotRequireOwnership(operation, args);
            }
        }

        private void DoOperationImpl(Operation operation, object[] args)
        {
            switch (operation)
            {
                case Operation.Invalid:
                    throw new InvalidOperationException();
                case Operation.AddPlayer:
                    AddPlayer(args);
                    break;
                case Operation.RemovePlayer:
                    RemovePlayer(args);
                    break;
                case Operation.GetSelf:
                    GetSelf(args);
                    break;
                case Operation.AssignSelf:
                    AssignSelf(args);
                    break;
                case Operation.DrawCard:
                    DrawCard(args);
                    break;
                case Operation.PlayCard:
                    PlayCard(args);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        [ClientRpc]
        private void DoOperationClient(Operation operation, object[] args)
        {
            DoOperationImpl(operation, args);
        }

        [ServerRpc]
        private void DoOperationServerRequireOwnership(Operation operation, object[] args)
        {
            DoOperationImpl(operation, args);
        }

        [ServerRpc(RequireOwnership = false)]
        private void DoOperationServerNotRequireOwnership(Operation operation, object[] args)
        {
            DoOperationImpl(operation, args);
        }

        private void AddPlayer(object[] args)
        {
            if (args == null || args.Length != 1 || args[0] is not ulong)
            {
                throw new ArgumentException("AddPlayer takes exactly 1 ulong argument");
            }

            _gameManager.AddPlayerServerRpc((ulong)args[0]);
        }
        private void RemovePlayer(object[] args)
        {
            if (args == null || args.Length != 1 || args[0] is not ulong)
            {
                throw new ArgumentException("RemovePlayer takes exactly 1 ulong argument");
            }

            _gameManager.RemovePlayerServerRpc((ulong)args[0]);
        }
        private void GetSelf(object[] args)
        {
            if (args != null)
            {
                throw new ArgumentException("GetSelf takes no arguments");
            }

            _gameManager.GetSelfServerRpc(default);
        }
        private void AssignSelf(object[] args)
        {
            if (args == null || args.Length != 1 || args[0] is not ClientRpcParams)
            {
                throw new ArgumentException("AssignSelf takes exactly 1 ClientRpcParams argument");
            }

            _gameManager.AssignSelfClientRpc((ClientRpcParams)args[0]);
        }
        private void DrawCard(object[] args)
        {
            if (args != null)
            {
                throw new ArgumentException("DrawCard takes no arguments");
            }

            _gameManager.DrawCardServerRpc(default);
        }
        private void PlayCard(object[] args)
        {
            if (args == null || args.Length != 1 || args[0] is not SerializedPlayingCard)
            {
                throw new ArgumentException("PlayCard takes exactly 1 SerializedPlayingCard argument");
            }

            _gameManager.PlayCardServerRpc((SerializedPlayingCard)args[0], default);
        }
    }
}
