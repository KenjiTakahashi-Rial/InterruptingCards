using System;

using UnityEngine;

using InterruptingCards.Managers;

namespace InterruptingCards.Behaviours
{
    public class InitializingGameStateBehaviour : StateMachineBehaviour
    {
        // TODO: Try without Lazy and just use singleton in OnStateMachineEnter
        private readonly Lazy<AbstractGameManager> _gameManager = new(() => AbstractGameManager.Singleton);

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _gameManager.Value.HandleInitializeGame();
        }
    }
}
