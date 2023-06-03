using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Config;

namespace InterruptingCards.Managers
{
    public class StateMachineManager : NetworkBehaviour
    {
        private const int GameStateMachineLayer = 0;

        private readonly StateMachineConfig _stateMachineConfig = StateMachineConfig.Singleton;

        [SerializeField] private Animator _stateMachine;

        public StateMachine CurrentState => _stateMachineConfig.GetEnum(
            _stateMachine.GetCurrentAnimatorStateInfo(GameStateMachineLayer).fullPathHash
        );

        public string CurrentStateName => CurrentState.ToString();

        public void SetTrigger(StateMachine trigger)
        {
            if (IsServer)
            {
                SetTriggerClientRpc(trigger);
            }
            else if (!NetworkManager.IsConnectedClient)
            {
                SetTriggerImpl(trigger);
            }
            else
            {
                Debug.LogWarning($"Client attempted to change state while connected to host");
            }
        }

        [ClientRpc]
        private void SetTriggerClientRpc(StateMachine trigger)
        {
            SetTriggerImpl(trigger);
        }

        private void SetTriggerImpl(StateMachine trigger)
        {
            SetTriggerImpl(_stateMachineConfig.GetId(trigger));
        }

        private void SetTriggerImpl(int id)
        {
            Debug.Log($"Triggering {_stateMachineConfig.GetName(id)}");
            _stateMachine.SetTrigger(id);
        }
    }
}
