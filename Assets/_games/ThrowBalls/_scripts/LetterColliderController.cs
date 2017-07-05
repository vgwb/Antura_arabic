using UnityEngine;

namespace Antura.Minigames.ThrowBalls
{
    public class LetterColliderController : MonoBehaviour
    {
        public LetterController letterController;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnCollisionEnter(Collision collision)
        {
            letterController.OnCollisionEnter(collision);
        }
    }
}