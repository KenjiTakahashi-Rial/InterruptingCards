using System;

using UnityEngine;

using InterruptingCards.Managers;

namespace InterruptingCards.Behaviours
{
    public class StartTurnStateBehaviour : StateMachineBehaviour
    {
        // TODO: Think of a way to make this more generic (any kind of game manager)
        private readonly Lazy<AbstractGameManager> _gameManager = new(() => AbstractGameManager.Singleton);

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _gameManager.Value.HandleStartTurn();
        }
    }
}