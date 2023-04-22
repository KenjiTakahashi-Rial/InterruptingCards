namespace InterruptingCards.Managers.GameManagers
{
    // AbstractGameManager is generic, so it cannot inherit from NetworkBehaviour
    // Couple the concrete game manager with an implementation of this for networking capabilities
    public interface IGameManagerNetworkDependency
    {
        internal static IGameManagerNetworkDependency Singleton { get; }

        internal void DoOperation(Operation operation, object[] args = null, bool clientRpc = false, bool requireOwnership = true);
    }
}
