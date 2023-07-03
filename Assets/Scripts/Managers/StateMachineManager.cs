using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Config;

namespace InterruptingCards.Managers
{
    public class StateMachineManager : NetworkBehaviour
    {
        private const int GameStateMachineLayer = 0;

        private readonly StateMachineConfig _stateMachineConfig = StateMachineConfig.Singleton;
        private readonly Dictionary<int, int> _triggerCounts = new();

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
            SetTriggerImpl(_stateMachineConfig.GetId(trigger));
        }

        private void SetTriggerImpl(int id)
        {
            var name = _stateMachineConfig.GetName(id);

            if (_stateMachine.GetBool(id))
            {
                var count = _triggerCounts[id];
                Log.Info($"{name} has not been consumed. Incrementing trigger count to {count + 1}");
                _triggerCounts[id]++;
                StartCoroutine(AutoResetTrigger(id));
            }

            Log.Info($"Setting trigger {name} from {CurrentState}");
            _stateMachine.SetTrigger(id);
        }

        private IEnumerator AutoResetTrigger(int id)
        {
            while (_stateMachine.GetBool(id))
            {
                yield return null;
            }

            _stateMachine.SetTrigger(id);
            
            if (_triggerCounts[id] == 1)
            {
                _triggerCounts.Remove(id);
            }
            else
            {
                _triggerCounts[id]--;
            }
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
            Log.Info($"Setting bool {_stateMachineConfig.GetName(id)} {val} from {CurrentState}");
            _stateMachine.SetBool(id, val);
        }
    }
}
