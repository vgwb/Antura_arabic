using UnityEngine;

public enum AnturaAnimationStates
{
    idle,
    sitting,
    sleeping,
    sheeping,
    sucking

}

public class AnturaAnimationController : MonoBehaviour
{
    AnturaAnimationStates state = AnturaAnimationStates.idle;
    public AnturaAnimationStates State
    {
        get { return state; }
        set
        {
            if (state != value)
            {
                var oldState = state;
                state = value;
                OnStateChanged(oldState, state);
            }
        }
    }

    float walkingSpeed;
    public float WalkingSpeed
    {
        get
        {
            return walkingSpeed;
        }
        set
        {
            walkingSpeed = value;
            animator.SetFloat("walkSpeed", value);
        }
    }

    bool isAngry;
    public bool IsAngry
    {
        get
        {
            return isAngry;
        }
        set
        {
            isAngry = value;
            animator.SetBool("angry", value);
        }
    }


    bool isExcited;
    public bool IsExcited
    {
        get
        {
            return isExcited;
        }
        set
        {
            isExcited = value;
            animator.SetBool("excited", value);
        }
    }

    public void DoBark(System.Action onCompleted = null)
    {
        animator.SetTrigger("doBark");
    }

    public void DoSniff(System.Action onCompleted = null)
    {
        animator.SetTrigger("doSniff");
    }

    public void DoShout(System.Action onCompleted = null)
    {
        animator.SetTrigger("doShout");
    }

    public void DoBurp(System.Action onCompleted = null)
    {
        animator.SetTrigger("doBurp");
    }

    public void DoSpit(System.Action onCompleted = null)
    {
        animator.SetTrigger("doSpit");
    }

    public void OnJumpStart()
    {
        animator.SetBool("jumping", true);
        animator.SetBool("falling", true);
    }

    // when Antura grabs something in the air
    public void OnJumpGrab()
    {
        animator.SetTrigger("doAirGrab");
    }

    public void OnJumpMaximumHeightReached()
    {
        animator.SetBool("jumping", false);
        animator.SetBool("falling", true);
    }

    public void OnJumpEnded()
    {
        animator.SetBool("jumping", false);
        animator.SetBool("falling", false);
    }

    private Animator animator_;
    Animator animator
    {
        get
        {
            if (!animator_)
                animator_ = GetComponentInChildren<Animator>();
            return animator_;
        }
    }

    void OnStateChanged(AnturaAnimationStates oldState, AnturaAnimationStates newState)
    {
        animator.SetBool("idle", true);
        animator.SetBool("sitting", false);
        animator.SetBool("sleeping", false);
        animator.SetBool("sheeping", false);
        animator.SetBool("sucking", false);
        
        switch (newState)
        {
            case AnturaAnimationStates.idle:
                animator.SetBool("idle", true);
                break;
            case AnturaAnimationStates.sitting:
                animator.SetBool("sitting", true);
                break;
            case AnturaAnimationStates.sleeping:
                animator.SetBool("sleeping", true);
                break;
            case AnturaAnimationStates.sheeping:
                animator.SetBool("sheeping", true);
                break;
            case AnturaAnimationStates.sucking:
                animator.SetBool("sucking", true);
                break;
            default:
                // No specific visual behaviour for this state
                break;

        }
    }
}
