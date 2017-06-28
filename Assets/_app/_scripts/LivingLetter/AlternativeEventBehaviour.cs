using UnityEngine;

namespace EA4S.LivingLetters
{ 
    // refactor: group these behaviours in a folder
    public class AlternativeEventBehaviour : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<LivingLetterController>().OnIdleAlternativeEnter();
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<LivingLetterController>().OnIdleAlternativeExit();
        }
    }
}