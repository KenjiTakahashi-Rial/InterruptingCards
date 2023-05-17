namespace InterruptingCards.Config
{
    public enum StateMachine
    {
        Invalid,

        // States
        WaitingForClientsState,
        WaitingForAllReadyState,
        EndGameState,
        StartTurnState,
        WaitingForDrawCardState,
        WaitingForPlayCardState,
        EndTurnState,

        // Triggers
        DrawCardTrigger,
        ForceEndTurnTrigger,
        ForceEndGameTrigger,
        PlayCardTrigger,
        PlayCardActiveEffectTrigger,
        StartGameTrigger,
        WaitForReadyTrigger,
    }
}
