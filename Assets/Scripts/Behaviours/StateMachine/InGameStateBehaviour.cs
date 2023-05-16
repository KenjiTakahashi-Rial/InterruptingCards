using System;

using UnityEngine;

using InterruptingCards.Managers;

namespace InterruptingCards.Behaviours
{
    public class InGameStateBehaviour : StateMachineBehaviour
    {
        private readonly Lazy<AbstractGameManager> _gameManager = new(() => AbstractGameManager.Singleton);

        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            _gameManager.Value.HandleInGame();
        }
    }
}
