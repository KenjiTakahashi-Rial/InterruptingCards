using UnityEngine;

using InterruptingCards.Managers;

namespace InterruptingCards.Behaviours
{
    public class RechargingStateBehaviour : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GameManager.Singleton.RechargeStep();
        }
    }
}