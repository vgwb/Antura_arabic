using UnityEngine;
using System.Collections;

namespace EA4S.ThrowBalls
{
    public class TrailRendererController : MonoBehaviour
    {
        public TrailRenderer trailRenderer;
        private bool isFollowingPokeball = false;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (isFollowingPokeball)
            {
                transform.position = PokeballController.instance.transform.position;
            }
        }

        public void Reset()
        {
            trailRenderer.Clear();
        }

        public void SetIsFollowPokeball(bool isFollowingPokeball)
        {
            this.isFollowingPokeball = isFollowingPokeball;
        }
    }
}
