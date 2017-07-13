using Antura.Audio;
using UnityEngine;

namespace Antura.Map
{
    public class MapDot : MonoBehaviour
    {
        // TODO: these should NOT be set by hand
        public int learningBlockActual;
        public int playSessionActual;
        public int pos;

        public Material blackDot;
        public Material redDot;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                ChangeMaterialPinToRed();
                if (other.gameObject.GetComponent<PlayerPin>().playerOverDotPin)
                {
                    AudioManager.I.PlaySound(Sfx.UIButtonClick);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
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