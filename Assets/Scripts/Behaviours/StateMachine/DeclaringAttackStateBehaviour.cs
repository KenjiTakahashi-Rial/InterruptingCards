using UnityEngine;

using InterruptingCards.Managers;

namespace InterruptingCards.Behaviours
{
    public class DeclaringAttackStateBehaviour : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GameManager.Singleton.DeclareAttack();
        }
    }
}