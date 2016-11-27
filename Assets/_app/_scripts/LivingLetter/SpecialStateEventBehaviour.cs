using UnityEngine;
using System.Collections;
using EA4S;

public class SpecialStateEventBehaviour : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.SendMessage("OnActionCompleted");
    }
}
