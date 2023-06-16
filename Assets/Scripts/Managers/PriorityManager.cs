using System.Linq;

using Unity.Netcode;

using InterruptingCards.Behaviours;
using InterruptingCards.Config;
using InterruptingCards.Managers.TheStack;

namespace InterruptingCards.Managers
{
    public class PriorityManager : NetworkBehaviour
    {
        // TODO: Allow each player to set this individually
        private const bool AutoPass = true;

        private readonly NetworkVariable<ulong> _priorityPlayerId = new();

        private GameManager Game => GameManager.Singleton;
        private PlayerManager PlayerManager => Game.PlayerManager;
        private StateMachineManager TheStackStateMachineManager => Game.TheStackStateMachineManager;
        private TheStackManager TheStackManager => Game.TheStackManager;

        public PlayerBehaviour PriorityPlayer => PlayerManager[_priorityPlayerId.Value];

        // TODO: Add other factors (ability to purchase, etc.)
        public bool PriorityPlayerHasActions =>
            PriorityPlayer.LootPlays > 0 && !PriorityPlayer.ActivatedCards.Any(c => !c.IsDeactivated);

        private LogManager Log => LogManager.Singleton;

        public void OnNeworkSpawned()
        {
            if (IsServer)
            {
                PlayerManager.OnActivePlayerChanged += SetPlayerPriority;
                TheStackManager.OnResolve += SetPlayerPriority;
            }
        }

        public void OnNetworkDespawned()
        {
            if (IsServer)
            {
                PlayerManager.OnActivePlayerChanged -= SetPlayerPriority;
                TheStackManager.OnResolve -= SetPlayerPriority;
            }
        }

        public void PassPriority()
        {
            if (!IsServer)
            {
                Log.Warn($"Cannot pass priority if not host");
                return;
            }

            var theStackState = TheStackStateMachineManager.CurrentState;
            if (theStackState != StateMachine.TheStackPriorityPassing)
            {
                Log.Warn($"Cannot pass priority from {theStackState}");
            }

            var prevPriorityPlayer = PriorityPlayer;
            PriorityPlayer.LootPlays = PriorityPlayer == PlayerManager.ActivePlayer ? PriorityPlayer.LootPlays : 0;
            _priorityPlayerId.Value = PlayerManager.GetNextId(_priorityPlayerId.Value);
            Log.Info($"Passed priority from {prevPriorityPlayer.Name} to {PriorityPlayer.Name}");

            var lastPushBy = TheStackManager.LastPushBy;
            if (PriorityPlayer == lastPushBy || lastPushBy == null && PriorityPlayer == PlayerManager.ActivePlayer)
            {
                TheStackStateMachineManager.SetTrigger(StateMachine.TheStackPriorityPassComplete);
                return;
            }

            TryAutoPass();
        }

        public void TryAutoPass()
        {
            if (AutoPass && !PriorityPlayerHasActions)
            {
                Log.Info($"Automatically passing priority from {PriorityPlayer.Name}");
                PassPriority();
            }
        }

        private void SetPlayerPriority(PlayerBehaviour player)
        {
            _priorityPlayerId.Value = player.Id;
        }

        private void SetPlayerPriority(TheStackElement element)
        {
            _priorityPlayerId.Value = element.PushedById;
        }
    }
}