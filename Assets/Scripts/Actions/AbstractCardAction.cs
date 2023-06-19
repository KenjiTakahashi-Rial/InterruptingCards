using Unity.Netcode;

using InterruptingCards.Managers;

namespace InterruptingCards.Actions
{
    public abstract class AbstractCardAction : NetworkBehaviour
    {
        protected GameManager Game => GameManager.Singleton;
        protected LogManager Log => LogManager.Singleton;
        protected PlayerManager PlayerManager => Game.PlayerManager;
        protected PriorityManager PriorityManager => Game.PriorityManager;
        protected StateMachineManager GameStateMachineManager => Game.StateMachineManager;
        protected StateMachineManager TheStackStateMachineManager => Game.TheStackStateMachineManager;

        public void TryExecute(int cardId)
        {
            if (CanExecute(NetworkManager.LocalClientId, cardId))
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