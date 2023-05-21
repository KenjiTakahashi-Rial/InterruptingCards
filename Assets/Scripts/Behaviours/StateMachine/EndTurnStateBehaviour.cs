using UnityEngine;

using InterruptingCards.Managers;

namespace InterruptingCards.Behaviours
{
    public class EndTurnStateBehaviour : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            AbstractGameManager.Singleton.HandleEndTurn();
        }
    }
}
