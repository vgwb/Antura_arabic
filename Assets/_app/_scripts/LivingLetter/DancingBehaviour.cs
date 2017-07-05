using UnityEngine;

namespace Antura.LivingLetters
{
    // TODO refactor: group these behaviours in a folder
    public class DancingBehaviour : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.SendMessage("OnDancingStart");
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.SendMessage("OnDancingEnd");
        }
    }
}
