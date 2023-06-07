using Unity.Netcode;

namespace InterruptingCards.Actions
{
    public abstract class AbstractAction : NetworkBehaviour
    {
        protected abstract bool CanExecute { get; }

        public void TryExecute()
        {
            if (CanExecute)
            {
                Execute();
            }
        }

        protected abstract void Execute();

        [ServerRpc(RequireOwnership = false)]
        public void ExecuteServerRpc()
        {
            if (CanExecute)
            {
                Execute();
            }
        }
    }
}