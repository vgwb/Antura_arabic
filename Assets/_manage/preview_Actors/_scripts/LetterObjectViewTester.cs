using UnityEngine;
using System.Collections;
using EA4S;

public class LetterObjectViewTester : MonoBehaviour
{
    LetterObjectView letter;

    public LLAnimationStates targetState;
    public bool doTransition;

    public bool doHooray;
    public bool doAngry;
    public bool doHighFive;

    void Start ()
    {
        letter = GetComponent<LetterObjectView>();
	}
	
	void Update ()
    {
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
    }
}
