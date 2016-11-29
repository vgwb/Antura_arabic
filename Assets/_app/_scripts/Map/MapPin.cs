using UnityEngine;
using System.Collections;
using EA4S;

namespace EA4S
{
    public class MapPin : MonoBehaviour
    {
        public int Number;
        public bool unlocked;
        public Transform RopeNode;
        public GameObject Dot;
        public Material blackPin;
        public Material redPin;
        public int posBefore;

        void Start()
        {
            if (unlocked) Dot.SetActive(true);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                transform.GetChild(0).gameObject.SetActive(false);
                ChangeMaterialPinToRed();
            }
        }
        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
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