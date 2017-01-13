using UnityEngine;

// refactor: Helpers need to be standardized

public class AnimatorStateSelector : MonoBehaviour
{
    public string AnimatorState;

    void Start()
    {
        gameObject.GetComponent<Animator>().Play(AnimatorState);
    }

}