using UnityEngine;
using System.Collections;

public class AnimatorStateSelector : MonoBehaviour
{
    public string AnimatorState;

    void Start()
    {
        gameObject.GetComponent<Animator>().Play(AnimatorState);
    }

}