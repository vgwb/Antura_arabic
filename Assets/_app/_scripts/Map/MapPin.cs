using UnityEngine;
using EA4S.Audio;
using EA4S.Map;

namespace EA4S
{
    public class MapPin : MonoBehaviour
    {
        public int learningBlockPin;
        public int playSessionPin;
        public bool unlocked;
        public Transform RopeNode;
        public GameObject Dot;
        public Material blackPin;
        public Material redPin;
        public int pos;

        void Start()
        {
            if (unlocked) {
                Dot.SetActive(true);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player") {
                transform.GetChild(0).gameObject.SetActive(false);
                ChangeMaterialPinToRed();
                if (other.gameObject.GetComponent<LetterMovement>().playerOverDotPin == true) {
                    AudioManager.I.PlaySound(Sfx.UIButtonClick);
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player") {
                transform.GetChild(0).gameObject.SetActive(true);
                ChangeMaterialPinToBlack();
            }
        }

        public void ChangeMaterialPinToBlack()
        {
            Dot.GetComponent<Renderer>().material = blackPin;
        }

        public void ChangeMaterialPinToRed()
        {
            Dot.GetComponent<Renderer>().material = redPin;
        }
    }
}