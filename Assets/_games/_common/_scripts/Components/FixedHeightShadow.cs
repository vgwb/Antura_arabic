using System;
using UnityEngine;

namespace EA4S
{
    public class FixedHeightShadow : MonoBehaviour
    {
        public Transform toFollow;
        public float y = 0;

        void Update()
        {
            if (toFollow == null)
                return;

            var pos = transform.position;
            pos = toFollow.position;
            pos.y = y;
            transform.position = pos;
        }

        public void Initialize(Transform toFollow, float y)
        {
            this.toFollow = toFollow;
            this.y = y;
        }
    }
}
