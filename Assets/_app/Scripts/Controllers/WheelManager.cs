using UnityEngine;
using System.Collections;

namespace EA4S
{
    public class WheelManager : MonoBehaviour
    {
        public static WheelManager Instance;
        public GameObject Popup;

        void Awake() {
            Instance = this;
        }

        void Start() {
            Popup.SetActive(false);
        }

        public void OnWheenStopped() {
            Popup.SetActive(true);
        }

        // Update is called once per frame
        void Update() {
	
        }
    }
}