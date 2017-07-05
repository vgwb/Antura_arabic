using Antura.Audio;
using UnityEngine;

namespace Antura.Map
{
    public class Dot : MonoBehaviour
    {
        public int learningBlockActual;
        public int playSessionActual;
        public int pos;
        public Material blackDot;
        public Material redDot;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player") {
                ChangeMaterialPinToRed();
                if (other.gameObject.GetComponent<LetterMovement>().playerOverDotPin) {
                    AudioManager.I.PlaySound(Sfx.UIButtonClick);
                }
            }
        }
        
        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player") {
                ChangeMaterialPinToBlack();
            }
        }
        
        public void ChangeMaterialPinToBlack()
        {
            GetComponent<Renderer>().material = blackDot;
        }
        
        public void ChangeMaterialPinToRed()
        {
            GetComponent<Renderer>().material = redDot;
        }
    }
}
