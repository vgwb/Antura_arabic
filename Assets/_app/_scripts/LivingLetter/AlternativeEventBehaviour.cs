using UnityEngine;
using System.Collections;
using EA4S;

public class AlternativeEventBehaviour : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<LetterObjectView>().OnIdleAlternativeEnter();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<LetterObjectView>().OnIdleAlternativeExit();
    }
}
