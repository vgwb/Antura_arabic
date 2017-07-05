using UnityEngine;

namespace EA4S.Utilities
{
    public class AnimatorStateSelector : MonoBehaviour
    {
        public string AnimatorState;

        void Start()
        {
            gameObject.GetComponent<Animator>().Play(AnimatorState);
        }
    }
}