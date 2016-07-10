using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using DG.Tweening;

namespace EA4S
{

    public class WheelManager : MonoBehaviour
    {
        public static WheelManager Instance;
        public GameObject Popup;
        public GameObject GameIcon;

        public GameObject TutorialArrow;
        public GameObject GameTitle;

        public List<MinigameData> gameData = new List<MinigameData>();

        int currentGameIndex;
        Image PopupImage;

        int gameCounter;
        TextMeshProUGUI labelText;

        void Awake() {
            Instance = this;
            currentGameIndex = 0;
            PopupImage = Popup.GetComponent<Image>();
            labelText = GameTitle.GetComponent<TextMeshProUGUI>();

            gameData.Add(new MinigameData("fastcrowd", "الحشد سريع", "Fast Crowd", true));
            gameData.Add(new MinigameData("dontwakeup", "لا يستيقظون", "Don't Wake Up", true));
            gameData.Add(new MinigameData("balloons", "بالونات", "Balloons", true));
            gameData.Add(new MinigameData("pianowoof", "بيانو", "Piano Woof", true));
        }

        void Start() {
            //GameIcon.SetActive(false);
        }

        public void OnWheenStopped() {
            GameIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>(gameData[currentGameIndex].GetIconResourcePath());
            //GameIcon.SetActive(true);
        }

        public void OnRadiusTrigger(int number, Color _color) {
            if (number != currentGameIndex) {
                currentGameIndex = (number % 4);
                //Debug.Log("OnRadiusTrigger" + currentSector);

                PopupImage.color = _color;

                labelText.text = ArabicFixer.Fix(gameData[currentGameIndex].Title, false, false);
                GameIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>(gameData[currentGameIndex].GetIconResourcePath());
                AudioManager.PlaySound("SFX/hit");
            }


        }

    }
}