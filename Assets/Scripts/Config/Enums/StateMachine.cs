namespace InterruptingCards.Config
{
    // TODO: Make state tense consistent
    public enum StateMachine
    {
        Invalid,

        // States
        WaitingForClientsState,
        WaitingForAllReadyState,
        InitializingGameState,
        EndGameState,
        StartTurnState,
        WaitingForDrawCardState,
        WaitingForPlayCardState,
        EndTurnState,

        // Triggers
        AllReadyTrigger,
        DrawCardTrigger,
        ForceEndTurnTrigger,
        ForceEndGameTrigger,
        PlayCardTrigger,
        PlayCardActiveEffectTrigger,
        StartGameTrigger,
        WaitForReadyTrigger,
    }
}
