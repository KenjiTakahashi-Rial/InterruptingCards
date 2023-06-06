using UnityEngine;

namespace InterruptingCards.Managers
{
    public class ActionManager : MonoBehaviour
    {
        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private StateMachineManager _stateMachineManager;
        [SerializeField] private StateMachineManager _gameStateMachineManager;
    }
}