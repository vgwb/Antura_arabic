using UnityEngine;
using System.Collections;
using EA4S;

public class LetterObjectViewTester : MonoBehaviour
{
    LetterObjectView letter;

    public LLAnimationStates targetState;
    public bool doTransition;

    [Range(0,1)]
    public float walkSpeed;

    public bool fear;

    public bool doHooray;
    public bool doAngry;
    public bool doHighFive;

    public bool onJumpStart;
    public bool onJumpMiddle;
    public bool onJumpEnd;
    public bool doSmallJump;

    public bool doDanceWin;
    public bool doDanceLose;
    public bool doDanceTwirl;
    public bool doToggleDance;

    void Start ()
    {
        letter = GetComponent<LetterObjectView>();
	}
	
	void Update ()
    {
        letter.HasFear = fear;
        letter.SetWalkingSpeed(walkSpeed);

	    if (doTransition)
        {
            doTransition = false;
            letter.SetState(targetState);
        }

        if (doHooray)
        {
            doHooray = false;
            letter.DoHorray();
        }


        if (doAngry)
        {
            doAngry = false;
            letter.DoAngry();
        }


        if (doHighFive)
        {
            doHighFive = false;
            letter.DoHighFive();
        }


        if (onJumpStart)
        {
            onJumpStart = false;
            letter.OnJumpStart();
        }


        if (onJumpMiddle)
        {
            onJumpMiddle = false;
            letter.OnJumpMaximumHeightReached();
        }



        if (onJumpEnd)
        {
            onJumpEnd = false;
            letter.OnJumpEnded();
        }

        if (doSmallJump)
        {
            doSmallJump = false;
            letter.DoSmallJump();
        }

        if (doDanceWin)
        {
            doDanceWin = false;
            letter.DoDancingWin();
        }

        if (doDanceLose)
        {
            doDanceLose = false;
            letter.DoDancingLose();
        }

        if (doDanceTwirl)
        {
            doDanceTwirl = false;
            letter.DoDancingTwirl(() => { Debug.Log("BACK!"); });
        }

        if (doToggleDance)
        {
            doToggleDance = false;
            letter.ToggleDance();
        }
    }
}
