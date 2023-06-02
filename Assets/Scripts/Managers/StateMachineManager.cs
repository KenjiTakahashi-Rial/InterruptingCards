using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Config;

namespace InterruptingCards.Managers
{
    public class StateMachineManager : NetworkBehaviour
    {
        private const int GameStateMachineLayer = 0;

        private readonly StateMachineConfig _stateMachineConfig = StateMachineConfig.Singleton;

        [SerializeField] private Animator _gameStateMachine;

        public StateMachine CurrentState => _stateMachineConfig.GetEnum(
            _gameStateMachine.GetCurrentAnimatorStateInfo(GameStateMachineLayer).fullPathHash
        );

        public string CurrentStateName => CurrentState.ToString();

        public void SetTrigger(int id)
        {
            _gameStateMachine.SetTrigger(id);
        }

        public void SetTrigger(StateMachine trigger)
        {
            SetTrigger(_stateMachineConfig.GetId(trigger));
        }

        [ClientRpc]
        public void SetTriggerClientRpc(StateMachine trigger)
        {
            Debug.Log($"Triggering {trigger}");
            SetTrigger(trigger);
        }
    }
}
