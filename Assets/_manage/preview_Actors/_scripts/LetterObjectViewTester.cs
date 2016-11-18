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

    [Range(0.5f, 2)]
    public float danceSpeed = 1;

    public bool fear;

    public bool doHooray;
    public bool doAngry;
    public bool doHighFive;

    public bool onJumpStart;
    public bool onJumpMiddle;
    public bool onJumpEnd;
    public bool doSmallJump;

    public bool doTwirl;

    public bool doDanceWin;
    public bool doDanceLose;
    public bool doToggleDance;

    void Start ()
    {
        letter = GetComponent<LetterObjectView>();
	}
	
	void Update ()
    {
        letter.HasFear = fear;
        letter.SetWalkingSpeed(walkSpeed);
        letter.SetDancingSpeed(danceSpeed);

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

        if (doTwirl)
        {
            doTwirl = false;
            letter.DoDancingTwirl(() => { Debug.Log("BACK!"); });
        }

        if (doToggleDance)
        {
            doToggleDance = false;
            letter.ToggleDance();
        }
    }
}
