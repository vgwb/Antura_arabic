using UnityEngine;
using System.Collections;

public class ReadingGameAntura : MonoBehaviour
{
    AnturaAnimationController anim;

    public bool angry = false;

	void Awake ()
    {
        anim = GetComponent<AnturaAnimationController>();
    }
	
	void Update ()
    {
        anim.IsAngry = angry;
        SetSitting(true);
    }

    public void SetSitting(bool sitting)
    {
        // emp
        if (sitting)
            anim.State = AnturaAnimationStates.sitting;
        else
            anim.State = AnturaAnimationStates.idle;
    }
}
