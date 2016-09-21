using UnityEngine;
using System.Collections;

namespace EA4S
{
    public class WheelRadius : MonoBehaviour
    {
        public Color color;
        public int number;

        void OnTriggerEnter2D(Collider2D other)
        {
            //        Debug.Log("Radius Trigger A " + other.gameObject.name + " " + number);
            if (other.gameObject.name == "Trigger") {
                WheelManager.Instance.OnRadiusTrigger(number, color);

            }
        }

    }
}