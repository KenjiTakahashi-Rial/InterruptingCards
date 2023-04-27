namespace InterruptingCards.Managers
{
    public interface IGameManager
    {
        static IGameManager Singleton { get; }

        void StartGame();
    }
}
