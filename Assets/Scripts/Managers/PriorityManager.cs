using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public class PriorityManager : MonoBehaviour
    {
        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private StateMachineManager _gameStateMachineManager;
        [SerializeField] private StateMachineManager _theStackStateMachineManager;
        [SerializeField] private TheStackManager _theStackManager;

        public Player PriorityPlayer { get; private set; }

        private LogManager Log => LogManager.Singleton;

        public void Awake()
        {
            _playerManager.OnActivePlayerChanged += SetPlayerPriority;
            _theStackManager.OnResolve += SetPlayerPriority;
        }

        public void OnDestroy()
        {
            _playerManager.OnActivePlayerChanged -= SetPlayerPriority;
            _theStackManager.OnResolve -= SetPlayerPriority;
        }

        public void PassPriority()
        {
            var prevPriorityPlayer = PriorityPlayer;
            PriorityPlayer = _playerManager.GetNext(PriorityPlayer.Id);
            var nextPriorityPlayer = PriorityPlayer;
            Log.Info($"Passing priority from {prevPriorityPlayer.Name} to {nextPriorityPlayer.Name}");

            var lastPushBy = _theStackManager.LastPushBy;
            if (PriorityPlayer == lastPushBy || lastPushBy == null && PriorityPlayer == _playerManager.ActivePlayer)
            {
                var theStackState = _theStackStateMachineManager.CurrentState;

                if (theStackState == StateMachine.TheStackPriorityPassing)
                {
                    _theStackStateMachineManager.SetTrigger(StateMachine.TheStackPriorityPassing);
                }
                else
                {
                    Log.Warn($"Cannot pass priority from {theStackState}");
                }
            }
        }

        private void SetPlayerPriority(Player player)
        {
            PriorityPlayer = player;
        }

        private void SetPlayerPriority(ITheStackElement element)
        {
            PriorityPlayer = element.PushedBy;
        }
    }
}