using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Managers.TheStack;
using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public class PriorityManager : NetworkBehaviour
    {
        private readonly NetworkVariable<ulong> _priorityPlayerId = new();

        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private StateMachineManager _gameStateMachineManager;
        [SerializeField] private StateMachineManager _theStackStateMachineManager;
        [SerializeField] private TheStackManager _theStackManager;

        public Player PriorityPlayer => _playerManager[_priorityPlayerId.Value];

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
            var nextPriorityPlayer = PriorityPlayer;
            Log.Info($"Passing priority from {prevPriorityPlayer.Name} to {nextPriorityPlayer.Name}");

            var lastPushBy = _theStackManager.LastPushBy;
            if (PriorityPlayer == lastPushBy || lastPushBy == null && PriorityPlayer == _playerManager.ActivePlayer)
            {
                _theStackStateMachineManager.SetTrigger(StateMachine.TheStackPriorityPassComplete);
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