using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ModularFramework.Core;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using DG.Tweening;

namespace EA4S
{

    public class WheelManager : MonoBehaviour
    {
        [Header("Scene Setup")]
        public Music SceneMusic;

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
            AudioManager.I.PlayMusic(SceneMusic);

            WidgetSubtitles.I.DisplaySentence("wheel_turn", 2, true);

        }

        public void OnPopuplicked() {
            Debug.Log("Wheel start game: " + gameData[currentGameIndex].Code);
            if (isGameSelected) {
                if (gameData[currentGameIndex].Code == "fastcrowd" || gameData[currentGameIndex].Code == "fastcrowd_words") {
                    FastCrowd.FastCrowdGameplayInfo gameplayInfo = new FastCrowd.FastCrowdGameplayInfo();
                    gameplayInfo.Variant = FastCrowd.FastCrowdGameplayInfo.GameVariant.living_letters;
                    GameManager.Instance.Modules.GameplayModule.GameplayStart(gameplayInfo);
                }
                GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition(gameData[currentGameIndex].SceneName);
                //SceneManager.LoadScene(gameData[currentGameIndex].SceneName);
            }
        }

        void ShakePopup() {
            AudioManager.I.PlaySfx(Sfx.UIPopup);
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
            AudioManager.I.PlaySfx(Sfx.WheelStart);

            //AudioManager.I.PlayMusic("Music2");
        }

        public void OnWheelStopped() {
            AudioManager.I.StopSfx(Sfx.WheelStart);
            GameIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>(gameData[currentGameIndex].GetIconResourcePath());
            AudioManager.I.PlayMusic(Music.Relax);
            isGameSelected = true;
            ShakePopup();

            ContinueScreen.Show(OnPopuplicked, ContinueScreenMode.ButtonWithBgFullscreen);
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
                    AudioManager.I.PlaySfx(Sfx.WheelTick);
                    //AudioManager.I.PlayHit();
                }
            }
        }
    }
}