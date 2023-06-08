using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Managers;

namespace InterruptingCards.Actions
{
    public class PassPriorityAction : AbstractAction
    {
        [SerializeField] private PriorityManager _priorityManager;
        [SerializeField] private StateMachineManager _theStackStateMachineManager;

        protected override bool CanExecute(ulong playerId)
        {
            if (playerId != _priorityManager.PriorityPlayer.Id)
            {
                Debug.LogWarning(
                    $"Cannot pass priority if not priority player (self: {_playerManager.SelfId}, priority player: " +
                    $"{_priorityManager.PriorityPlayer.Name})"
                );
                return false;
            }

            var theStackState = _theStackStateMachineManager.CurrentState;
            if (theStackState != StateMachine.TheStackPriorityPassing)
            {
                Debug.LogWarning($"Cannot pass priority from {theStackState}");
                return false;
            }

            return true;
        }

        protected override void Execute()
        {
            _priorityManager.PassPriority();
        }
    }
}