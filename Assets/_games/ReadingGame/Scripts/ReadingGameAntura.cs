using UnityEngine;
using System.Collections;

public class ReadingGameAntura : MonoBehaviour
{
    AnturaAnimationController anim;

    public bool angry = false;

	void Awake ()
    {
        anim = GetComponent<AnturaAnimationController>();
        anim.State = AnturaAnimationStates.sitting;
    }
	
	void Update ()
    {
        anim.IsAngry = angry;
    }
}
