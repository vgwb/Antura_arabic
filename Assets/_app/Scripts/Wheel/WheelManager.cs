using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace EA4S
{
    public class WheelManager : MonoBehaviour
    {
        public static WheelManager Instance;
        public GameObject Popup;
        public GameObject GameIcon;

        int currentSector;
        Image PopupImage;

        void Awake() {
            Instance = this;
            currentSector = 0;
            PopupImage = Popup.GetComponent<Image>();
        }

        void Start() {
            GameIcon.SetActive(false);
        }

        public void OnWheenStopped() {
            GameIcon.SetActive(true);
        }

        public void OnRadiusTrigger(int number, Color _color) {
            if (number != currentSector) {
                currentSector = number;
                //Debug.Log("OnRadiusTrigger" + currentSector);

                PopupImage.color = _color;

                AudioManager.PlaySound("SFX/hit");
            }


        }

        // Update is called once per frame
        void Update() {
	
        }
    }
}