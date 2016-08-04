using UnityEngine;
using System.Collections;

namespace EA4S.DontWakeUp
{
    public class DangerDog : MonoBehaviour
    {
        public static DangerDog I;
        public GameObject[] DangerZones;

        void Awake()
        {
            I = this;
            Hide();
        }

        public void Show()
        {
            foreach (var go in DangerZones) {
                go.SetActive(true);
            }
                
        }

        public void Hide()
        {
            foreach (var go in DangerZones) {
                go.SetActive(false);
            }
        }
    }
}