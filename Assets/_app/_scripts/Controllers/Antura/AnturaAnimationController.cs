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

        }
    }

    public void DoBark(System.Action onCompleted = null)
    {

    }

    public void DoSniff(System.Action onCompleted = null)
    {

    }

    public void DoShout(System.Action onCompleted = null)
    {

    }

    public void DoBurp(System.Action onCompleted = null)
    {

    }

    public void DoSpit(System.Action onCompleted = null)
    {

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

    }
}
