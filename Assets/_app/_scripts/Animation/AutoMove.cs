using UnityEngine;

namespace Antura.Animation
{
    // TODO refactor: this can be moved to the Animation utilities.
    public class AutoMove : MonoBehaviour
    {
        public Vector3 velocity = new Vector3(-1, -0.1f, -0.2f);
        public float speedFactor = 1;
    
        public void SetTime(float t)
        {
            transform.position += speedFactor * velocity * Time.deltaTime;
        }
    }
}

