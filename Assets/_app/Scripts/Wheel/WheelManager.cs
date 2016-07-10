using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
        public WheelController WheelCntrl;

        public GameObject Popup;
        public GameObject GameIcon;

        public GameObject TutorialArrow;
        public GameObject GameTitle;

        public List<MinigameData> gameData = new List<MinigameData>();

        int currentGameIndex;
        Image PopupImage;

        int gameCounter;
        TextMeshProUGUI labelText;

        bool isGameSelected;

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
            isGameSelected = false;
            //AudioManager.I.PlaySound("Music/play");
            AudioManager.I.PlayMusic("Music1");
            //AudioManager.I.PlayMusic();

            //ShakePopup();

        }

        public void OnPopuplicked() {
            if (isGameSelected) {
                SceneManager.LoadScene("game_DontWakeUp");
            }
        }

        void ShakePopup() {
            Tween BobiBobi = Popup.GetComponent<RectTransform>().DOPunchScale(new Vector2(1.0f, 1.0f), 0.8f, 5, 1f)
                .SetLoops(0).SetUpdate(true).SetAutoKill(true)
                .SetEase(Ease.InOutQuad).Play();

            //BobiBobi = PopupRect.DOSizeDelta(new Vector2(5, 5), 2f).SetRelative().SetUpdate(true).SetAutoKill(false).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);

            //            BobiBobi = Popup.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, -20), 0.6f).SetRelative().SetUpdate(true).SetAutoKill(false)
            //                .SetEase(Ease.InOutQuad).SetLoops(2, LoopType.Yoyo).Play();
        }


        public void OnWheelStart() {
            TutorialArrow.SetActive(false);
            //AudioManager.I.PlayMusic("Music2");
        }

        public void OnWheelStopped() {
            GameIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>(gameData[currentGameIndex].GetIconResourcePath());
            AudioManager.I.PlayMusic("Music2");
            isGameSelected = true;
            ShakePopup();
            // AudioManager.I.PlayMusic2();
            //GameIcon.SetActive(true);
        }

        public void OnRadiusTrigger(int number, Color _color) {
            if (WheelCntrl.isRotating) {
                if (number != currentGameIndex) {
                    currentGameIndex = (number % 4);
                    //Debug.Log("OnRadiusTrigger" + currentSector);

                    PopupImage.color = _color;

                    labelText.text = ArabicFixer.Fix(gameData[currentGameIndex].Title, false, false);
                    GameIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>(gameData[currentGameIndex].GetIconResourcePath());
                    AudioManager.I.PlaySound("SFX/hit");
                    //AudioManager.I.PlayHit();
                }
            }
        }
    }
}