namespace InterruptingCards.Managers.GameManagers
{
    // AbstractGameManager is generic, so it cannot inherit from NetworkBehaviour
    // Couple the concrete game manager with this decorator to provide networking capabilities
    public interface IGameManagerNetworkingDecorator
    {
        internal void DoOperation(Operation operation, object[] args, bool clientRpc = false, bool requireOwnership = true);
    }
}
