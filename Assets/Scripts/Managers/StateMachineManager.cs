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

        private LogManager Log => LogManager.Singleton;

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
                Log.Warn($"Cannot set trigger {trigger} while connected to host");
            }
        }

        [ClientRpc]
        private void SetTriggerClientRpc(StateMachine trigger)
        {
            SetTriggerImpl(trigger);
        }

        private void SetTriggerImpl(StateMachine trigger)
        {
            var id = _stateMachineConfig.GetId(trigger);
            Log.Info($"Triggering {_stateMachineConfig.GetName(id)}");
            _stateMachine.SetTrigger(id);
        }

        public void SetBool(StateMachine param, bool val)
        {
            if (IsServer)
            {
                SetBoolClientRpc(param, val);
            }
            else if (!NetworkManager.IsConnectedClient)
            {
                SetBoolImpl(param, val);
            }
            else
            {
                Log.Warn($"Cannot set bool {param} while connected to host");
            }
        }

        [ClientRpc]
        private void SetBoolClientRpc(StateMachine param, bool val)
        {
            SetBoolImpl(param, val);
        }

        private void SetBoolImpl(StateMachine param, bool val)
        {
            var id = _stateMachineConfig.GetId(param);
            Log.Info($"Setting bool {_stateMachineConfig.GetName(id)} {val}");
            _stateMachine.SetBool(id, val);
        }
    }
}
