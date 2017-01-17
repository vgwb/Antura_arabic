using UnityEngine;

namespace EA4S.LivingLetters
{
    // refactor: group these behaviours in a folder
    public class SpecialStateEventBehaviour : StateMachineBehaviour
    {
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.SendMessage("OnActionCompleted");
        }
    }
}
