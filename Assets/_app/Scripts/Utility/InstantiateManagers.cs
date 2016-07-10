using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace EA4S
{
    public class InstantiateManagers : MonoBehaviour
    {
        public GameObject FabricAudioManager;
        public GameObject EventsManager;
        public GameObject TouchManager;

        void Awake() {
            if (FindObjectOfType(typeof(Fabric.EventManager)) == null) {
                Instantiate(FabricAudioManager);
            }

            if (FindObjectOfType(typeof(EventSystem)) == null) {
                Instantiate(EventsManager);
            }

            if (FindObjectOfType(typeof(Lean.LeanTouch)) == null) {
                Instantiate(TouchManager);
            }

        }
    }
}