using System;

using UnityEngine;

using InterruptingCards.Managers;

namespace InterruptingCards.Behaviours
{
    public class InGameStateBehaviour : StateMachineBehaviour
    {
        private readonly Lazy<IGameManager> _gameManager = new(() => BasicGameManager.Singleton);

        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            _gameManager.Value.StartGame();
        }
    }
}
