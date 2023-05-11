namespace InterruptingCards.Managers
{
    public interface IGameManager
    {
        static IGameManager Singleton { get; }

        void HandleInGame();

        void HandleStartTurn();

        void HandleEndTurn(int shifts = 1);

        void HandleEndGame();
    }
}
