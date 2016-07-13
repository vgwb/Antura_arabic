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

        int numberOfGames;
        public GameObject Popup;
        public GameObject GameIcon;

        public GameObject TutorialArrow;
        public GameObject GameTitle;

        int currentGameIndex;
        Image PopupImage;

        int gameCounter;
        TextMeshProUGUI labelText;

        bool isGameSelected;

        List<MinigameData> gameData = new List<MinigameData>();

        void Awake() {
            Instance = this;
        }

        void Start() {
            AppManager.Instance.InitDataAI();
            gameData = AppManager.Instance.Teacher.GimmeGoodMinigames();
            numberOfGames = gameData.Count;

            currentGameIndex = 0;
            PopupImage = Popup.GetComponent<Image>();
            labelText = GameTitle.GetComponent<TextMeshProUGUI>();

            isGameSelected = false;
            AudioManager.I.PlayMusic("Music1");
        }

        public void OnPopuplicked() {
            if (isGameSelected) {
                SceneManager.LoadScene(gameData[currentGameIndex].SceneName);
            }
        }

        void ShakePopup() {
//            Sequence mySequence = DOTween.Sequence();
//            mySequence.Append(transform.DOMoveX(45, 1))
//                .Append(transform.DORotate(new Vector3(0,180,0), 1))
//                 .PrependInterval(1)
//                  .Insert(0, transform.DOScale(new Vector3(3,3,3), mySequence.Duration()));
//            
            Popup.GetComponent<RectTransform>().DOPunchScale(new Vector2(0.2f, 0.2f), 1.0f, 5, 1f)
                .SetLoops(0).SetUpdate(true).SetAutoKill(true)
                .SetEase(Ease.InOutQuad).Play();
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
                    currentGameIndex = (number % numberOfGames);
                    //Debug.Log("OnRadiusTrigger" + currentSector);

                    PopupImage.color = _color;

                    labelText.text = ArabicFixer.Fix(gameData[currentGameIndex].Title, false, false);
                    GameIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>(gameData[currentGameIndex].GetIconResourcePath());
                    AudioManager.I.PlaySound("Sfx/hit");
                    //AudioManager.I.PlayHit();
                }
            }
        }
    }
}