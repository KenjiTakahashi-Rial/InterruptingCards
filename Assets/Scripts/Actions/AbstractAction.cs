using Unity.Netcode;

using InterruptingCards.Managers;

namespace InterruptingCards.Actions
{
    public abstract class AbstractAction : NetworkBehaviour
    {
        protected GameManager Game => GameManager.Singleton;
        protected LogManager Log => LogManager.Singleton;
        protected PlayerManager PlayerManager => Game.PlayerManager;
        protected StateMachineManager GameStateMachineManager => Game.StateMachineManager;

        public void TryExecute()
        {
            if (CanExecute(NetworkManager.LocalClientId))
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