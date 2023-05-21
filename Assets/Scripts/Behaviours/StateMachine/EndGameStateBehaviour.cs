using InterruptingCards.Managers;

using UnityEngine;

namespace InterruptingCards.Behaviours
{
    public class EndGameStateBehaviour : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            AbstractGameManager.Singleton.HandleEndGame();
        }
    }
}
