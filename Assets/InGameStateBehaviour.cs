using System;

using UnityEngine;

using InterruptingCards.Managers.GameManagers;
using InterruptingCards.Models;

namespace InterruptingCards.Behaviours
{
    public class InGameStateBehaviour : StateMachineBehaviour
    {
        private readonly Lazy<IGameManager> _gameManager = new(() => PlayingCardGameManager.Singleton);

        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            _gameManager.Value.StartGame();
        }

        // OnStateMachineExit is called when exiting a state machine via its Exit Node
        //override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        //{
        //    
        //}
    }
}
