using UnityEngine;
using System.Collections;

namespace EA4S.ThrowBalls
{
    public class LetterColliderScript : MonoBehaviour
    {
        public LetterController parentController;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnCollisionEnter(Collision collision)
        {
            parentController.OnCollision(collision);
        }
    }
}
