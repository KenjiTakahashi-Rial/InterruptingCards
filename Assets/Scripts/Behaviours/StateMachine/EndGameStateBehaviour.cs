using System;

using InterruptingCards.Managers;

using UnityEngine;

namespace InterruptingCards.Behaviours
{
    public class EndGameStateBehaviour : StateMachineBehaviour
    {
        private readonly Lazy<IGameManager> _gameManager = new(() => AbstractGameManager.Singleton);

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _gameManager.Value.EndGame();
        }
    }
}
