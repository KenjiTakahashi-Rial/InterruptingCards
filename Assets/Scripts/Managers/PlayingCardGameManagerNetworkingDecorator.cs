using System;

using Unity.Netcode;

using InterruptingCards.Factories;
using InterruptingCards.Models;

namespace InterruptingCards.Managers.GameManagers
{
    public class PlayingCardGameManagerNetworkingDecorator : NetworkBehaviour, IGameManagerNetworkingDecorator
    {
        private readonly PlayingCardGameManager _gameManager = new(PlayingCardPlayerFactory.Singleton);

        public override void OnNetworkSpawn()
        {
            _gameManager.OnNetworkSpawn();
        }

        public override void OnNetworkDespawn()
        {
            _gameManager.OnNetworkDespawn();
        }

        void IGameManagerNetworkingDecorator.DoOperation(Operation operation, object[] args, bool clientRpc, bool requireOwnership)
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
                    if (args.Length != 1 || args[0] is not ulong)
                    {
                        throw new ArgumentException("AddPlayer takes exactly 1 ulong argument");
                    }

                    _gameManager.AddPlayerServerRpc((ulong)args[0]);

                    break;
                case Operation.RemovePlayer:
                    if (args.Length != 1 || args[0] is not ulong)
                    {
                        throw new ArgumentException("AddPlayer takes exactly 1 ulong argument");
                    }

                    _gameManager.RemovePlayerServerRpc((ulong)args[0]);

                    break;
                case Operation.DrawCard:
                    if (args.Length > 0)
                    {
                        throw new ArgumentException("DrawCard takes no arguments");
                    }

                    _gameManager.DrawCardServerRpc();

                    break;
                case Operation.PlayCard:
                    if (args.Length != 1 || args[0] is not ulong)
                    {
                        throw new ArgumentException("PlayCard takes exactly 1 ICard argument");
                    }

                    _gameManager.PlayCardServerRpc(args[0]); // TODO: Need to serialize instead of passing ICard type

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
    }
}
