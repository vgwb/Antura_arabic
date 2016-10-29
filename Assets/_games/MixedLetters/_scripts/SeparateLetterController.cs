using UnityEngine;
using System.Collections;
using TMPro;

namespace EA4S.MixedLetters
{
    public class SeparateLetterController : MonoBehaviour
    {
        public TMP_Text TMP_text;
        public Rigidbody rigidBody;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            rigidBody.AddForce(Constants.GRAVITY, ForceMode.Acceleration);
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void SetText(string letter)
        {
            TMP_text.SetText(letter);
        }
    }
}