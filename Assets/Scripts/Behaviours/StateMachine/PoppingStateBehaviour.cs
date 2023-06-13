using UnityEngine;

using InterruptingCards.Managers.TheStack;

namespace InterruptingCards.Behaviours
{
    public class PoppingStateBehaviour : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            TheStackManager.Singleton.Pop();
        }
    }
}