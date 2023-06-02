using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Config;
using Codice.Client.BaseCommands;

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

        public void SetTrigger(StateMachine trigger)
        {
            if (NetworkManager.IsListening)
            {
                SetTriggerClientRpc(trigger);
            }
            else
            {
                SetTriggerImpl(_stateMachineConfig.GetId(trigger));
            }
        }

        [ClientRpc]
        private void SetTriggerClientRpc(StateMachine trigger)
        {
            SetTrigger(trigger);
        }

        private void SetTriggerImpl(int id)
        {
            Debug.Log($"Triggering {_stateMachineConfig.GetName(id)}");
            _gameStateMachine.SetTrigger(id);
        }
    }
}
