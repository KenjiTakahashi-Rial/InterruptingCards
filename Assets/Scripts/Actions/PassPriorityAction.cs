using InterruptingCards.Config;
using InterruptingCards.Managers;

namespace InterruptingCards.Actions
{
    public class PassPriorityAction : AbstractAction
    {
        private PriorityManager PriorityManager => Game.PriorityManager;
        private StateMachineManager TheStackStateMachineManager => Game.TheStackStateMachineManager;

        protected override bool CanExecute(ulong playerId)
        {
            if (playerId != PriorityManager.PriorityPlayer.Id)
            {
                Log.Warn(
                    $"Cannot pass priority if not priority player (priority player: " +
                    $"{PriorityManager.PriorityPlayer.Name})"
                );
                return false;
            }

            var theStackState = TheStackStateMachineManager.CurrentState;
            if (theStackState != StateMachine.TheStackPriorityPassing)
            {
                Log.Warn($"Cannot pass priority from {theStackState}");
                return false;
            }

            return true;
        }

        protected override void Execute()
        {
            PriorityManager.PassPriority();
        }
    }
}