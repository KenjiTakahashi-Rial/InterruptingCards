using System;

using UnityEngine;

using InterruptingCards.Managers;

namespace InterruptingCards.Behaviours
{
    public class EndTurnStateBehaviour : StateMachineBehaviour
    {
        private readonly Lazy<AbstractGameManager> _gameManager = new(() => AbstractGameManager.Singleton);

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _gameManager.Value.HandleEndTurn();
        }
    }
}
