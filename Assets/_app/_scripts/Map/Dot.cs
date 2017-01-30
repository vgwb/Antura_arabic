using UnityEngine;
using System.Collections;
namespace EA4S
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
            if (other.gameObject.tag == "Player")
            {
                ChangeMaterialPinToRed();
            }
        }
        void OnTriggerExit(Collider other)
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
