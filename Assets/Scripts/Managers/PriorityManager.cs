using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Managers.TheStack;
using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public class PriorityManager : NetworkBehaviour
    {
        // TODO: Allow each player to set this individually
        private const bool AutoPass = true;

        private readonly NetworkVariable<ulong> _priorityPlayerId = new();

        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private StateMachineManager _gameStateMachineManager;
        [SerializeField] private StateMachineManager _theStackStateMachineManager;
        [SerializeField] private TheStackManager _theStackManager;

        public Player PriorityPlayer => _playerManager[_priorityPlayerId.Value];

        // TODO: Add other factors (ability to purchase, activate abilities, etc.)
        public bool PriorityPlayerHasActions => PriorityPlayer.LootPlays > 0;

        private LogManager Log => LogManager.Singleton;

        public void Awake()
        {
            _playerManager.OnActivePlayerChanged += SetPlayerPriority;
            _theStackManager.OnResolve += SetPlayerPriority;
        }

        public override void OnDestroy()
        {
            _playerManager.OnActivePlayerChanged -= SetPlayerPriority;
            _theStackManager.OnResolve -= SetPlayerPriority;
            base.OnDestroy();
        }

        public void PassPriority()
        {
            if (!IsServer)
            {
                Log.Warn($"Cannot pass priority if not host");
                return;
            }

            var theStackState = _theStackStateMachineManager.CurrentState;
            if (theStackState != StateMachine.TheStackPriorityPassing)
            {
                Log.Warn($"Cannot pass priority from {theStackState}");
            }

            var prevPriorityPlayer = PriorityPlayer;
            PriorityPlayer.LootPlays = 0;
            _priorityPlayerId.Value = _playerManager.GetNextId(_priorityPlayerId.Value);
            Log.Info($"Passed priority from {prevPriorityPlayer.Name} to {PriorityPlayer.Name}");

            var lastPushBy = _theStackManager.LastPushBy;
            if (PriorityPlayer == lastPushBy || lastPushBy == null && PriorityPlayer == _playerManager.ActivePlayer)
            {
                _theStackStateMachineManager.SetTrigger(StateMachine.TheStackPriorityPassComplete);
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

        private void SetPlayerPriority(Player player)
        {
            _priorityPlayerId.Value = player.Id;
        }

        private void SetPlayerPriority(TheStackElement element)
        {
            _priorityPlayerId.Value = element.PushedById;
        }
    }
}