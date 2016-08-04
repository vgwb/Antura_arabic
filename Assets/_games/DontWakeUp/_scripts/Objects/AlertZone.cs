using UnityEngine;
using System.Collections;

namespace EA4S.DontWakeUp
{
    public class AlertZone : MonoBehaviour
    {

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player") {
                DangerDog.I.Show();
            }

        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player") {
                DangerDog.I.Hide();
            }

        }
	
    }
}