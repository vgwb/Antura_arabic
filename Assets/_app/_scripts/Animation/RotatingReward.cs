using UnityEngine;

namespace EA4S.Animation
{
    public class RotatingReward : MonoBehaviour
    {
        public float speed = 10f;

        void Update()
        {
            transform.Rotate(Vector3.up, speed * Time.deltaTime);
        }
    }
}