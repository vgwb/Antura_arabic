using UnityEngine;
using System.Collections;

namespace EA4S
{
    public class AlertZone : MonoBehaviour
    {

        public GameObject DangerLine;

        void Start()
        {
            HideDangerLine();
        }

        void OnEnable()
        {
            HideDangerLine();
        }

        void OnDisable()
        {
            HideDangerLine();
        }

        public void HideDangerLine()
        {
            DangerLine.SetActive(false);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player") {
                DangerLine.SetActive(true);
            }

        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player") {
                DangerLine.SetActive(false);
            }

        }
	
    }
}