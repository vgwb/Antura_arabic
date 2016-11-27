using UnityEngine;
using System.Collections;
using EA4S;

public class AnturaWalkBehaviour : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<AnturaAnimationController>().SendMessage("OnAnimationWalkStart", SendMessageOptions.DontRequireReceiver);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<AnturaAnimationController>().SendMessage("OnAnimationWalkEnd", SendMessageOptions.DontRequireReceiver);
    }
}
