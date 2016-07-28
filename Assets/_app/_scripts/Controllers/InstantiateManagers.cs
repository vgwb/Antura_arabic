using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace EA4S
{
    public class InstantiateManagers : MonoBehaviour
    {
        public GameObject AudioManager;
        public GameObject EventsManager;
        public GameObject TouchManager;

        void Awake() {
            if (FindObjectOfType(typeof(AudioManager)) == null) {
                Instantiate(AudioManager);
            }

            if (FindObjectOfType(typeof(EventSystem)) == null) {
                Instantiate(EventsManager);
            }

            if (FindObjectOfType(typeof(Lean.LeanTouch)) == null) {
                Instantiate(TouchManager);
            }

            GlobalUI.Init();
        }
    }
}