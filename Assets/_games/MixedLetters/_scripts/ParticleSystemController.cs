using UnityEngine;

namespace Antura.Minigames.MixedLetters
{
    public class ParticleSystemController : MonoBehaviour
    {
        public static ParticleSystemController instance;

        new public ParticleSystem particleSystem;

        void Awake()
        {
            instance = this;
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void Reset()
        {
            particleSystem.time = 0;
        }

        public void SetPosition(Vector3 position)
        {
            SetPositionWithOffset(position, Vector3.zero);
        }

        public void SetPositionWithOffset(Vector3 position, Vector3 offset)
        {
            transform.position = position + offset;
        }
    }
}