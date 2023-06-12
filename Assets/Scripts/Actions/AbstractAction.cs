using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Managers;

namespace InterruptingCards.Actions
{
    public abstract class AbstractAction : NetworkBehaviour
    {
        [SerializeField] protected PlayerManager _playerManager;
        [SerializeField] protected StateMachineManager _gameStateMachineManager;

        protected LogManager Log => LogManager.Singleton;

        public void TryExecute()
        {
            if (CanExecute(_playerManager.SelfId))
            {
                ExecuteServerRpc();
            }
        }

        protected abstract bool CanExecute(ulong playerId);

        protected abstract void Execute();

        [ServerRpc(RequireOwnership = false)]
        protected void ExecuteServerRpc(ServerRpcParams serverRpcParams = default)
        {
            var senderId = serverRpcParams.Receive.SenderClientId;

            if (CanExecute(senderId))
            {
                Execute();
            }
        }
    }
}