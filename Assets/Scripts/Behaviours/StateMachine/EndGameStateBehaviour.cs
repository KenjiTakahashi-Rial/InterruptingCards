using System;

using InterruptingCards.Managers;

using UnityEngine;

namespace InterruptingCards.Behaviours
{
    public class EndGameStateBehaviour : StateMachineBehaviour
    {
        private readonly Lazy<IGameManager> _gameManager = new(() => BasicGameManager.Singleton);

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _gameManager.Value.HandleEndGame();
        }
    }
}
