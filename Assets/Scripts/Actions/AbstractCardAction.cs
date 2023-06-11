using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Managers;

namespace InterruptingCards.Actions
{
    public abstract class AbstractCardAction : NetworkBehaviour
    {
        [SerializeField] protected PlayerManager _playerManager;
        [SerializeField] protected StateMachineManager _gameStateMachineManager;

        protected LogManager Log => LogManager.Singleton;

        public void TryExecute(int cardId)
        {
            if (CanExecute(_playerManager.SelfId, cardId))
            {
                ExecuteServerRpc(cardId);
            }
        }

        protected abstract bool CanExecute(ulong playerId, int cardId);

        protected abstract void Execute(int cardId);

        [ServerRpc(RequireOwnership = false)]
        protected void ExecuteServerRpc(int cardId, ServerRpcParams serverRpcParams = default)
        {
            var senderId = serverRpcParams.Receive.SenderClientId;

            if (CanExecute(senderId, cardId))
            {
                Execute(cardId);
            }
        }
    }
}