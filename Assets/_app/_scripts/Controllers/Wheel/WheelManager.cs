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

        [Header("References")]
        public Antura AnturaController;
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
        /// <summary>
        /// Index (refered to gameData list) of the only game selectable for the actual playsession.
        /// </summary>
        int gameIndexToForceSelect;

        int tutorialIndex = 10;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            AppManager.Instance.InitDataAI();
            gameData = AppManager.Instance.Teacher.GimmeGoodMinigames();
            numberOfGames = gameData.Count;
            gameIndexToForceSelect = gameData.FindIndex(g => g.Code == AppManager.Instance.GetMiniGameForActualPlaySession().Code);

            currentGameIndex = 0;
            PopupImage = Popup.GetComponent<Image>();
            labelText = GameTitle.GetComponent<TextMeshProUGUI>();

            isGameSelected = false;
            AudioManager.I.PlayMusic(SceneMusic);

            showGameIcon(-1);

            Debug.Log("MapManager PlaySession " + AppManager.Instance.PlaySession);
            if (AppManager.Instance.PlaySessionGameDone >= 1) {
                tutorialIndex = 20;
            } else {
                tutorialIndex = 10;
            }

            AnturaController.SetPreset(AppManager.Instance.Player.AnturaCurrentPreset);

            SceneTransitioner.Close();
            ShowTutor();
        }

        public void ShowTutor()
        {
            switch (tutorialIndex) {
                case 10:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence("wheel_A1", 2, true, ShowTutor);
                    break;
                case 11:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence("wheel_A2", 1, true, ShowTutor);
                    break;
                case 12:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence("wheel_A3", 1, true, ShowTutor);
                    break;
                case 13:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence("wheel_A4", 2, true, ShowTutor);
                    break;
                case 14:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence("wheel_A5", 1, true);

                    break;
                case 20:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence("wheel_turn", 1, true);
                    break;
            }
        }

        void OnDisable()
        {
            StopSounds();
        }


        public void StopSounds()
        {
            AudioManager.I.StopSfx(Sfx.WheelStart);
        }

        public void OnPopuplicked()
        {
            /* Alpha static logic */
            if (isGameSelected) {
                MinigameData miniGame = AppManager.Instance.GetMiniGameForActualPlaySession();
                if (miniGame.Code == "fastcrowd" || miniGame.Code == "fastcrowd_words") {
                    FastCrowd.FastCrowdGameplayInfo gameplayInfo = new FastCrowd.FastCrowdGameplayInfo();
                    if (miniGame.Code == "fastcrowd") {
                        gameplayInfo.Variant = FastCrowd.FastCrowdGameplayInfo.GameVariant.living_letters;
                    } else {
                        gameplayInfo.Variant = FastCrowd.FastCrowdGameplayInfo.GameVariant.living_words;
                    }
                    GameManager.Instance.Modules.GameplayModule.GameplayStart(gameplayInfo);
                }
                if (miniGame.Code == "fastcrowd_words") {
                    GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_FastCrowd_tutorialWords");
                } else {
                    GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition(miniGame.SceneName + "_tutorial");
                }
            }
            /*
            Debug.Log("Wheel start game: " + gameData[currentGameIndex].Code);
            if (isGameSelected) {
                if (gameData[currentGameIndex].Code == "fastcrowd" || gameData[currentGameIndex].Code == "fastcrowd_words") {
                    FastCrowd.FastCrowdGameplayInfo gameplayInfo = new FastCrowd.FastCrowdGameplayInfo();
                    gameplayInfo.Variant = FastCrowd.FastCrowdGameplayInfo.GameVariant.living_letters;
                    GameManager.Instance.Modules.GameplayModule.GameplayStart(gameplayInfo);
                }
                CloseScene();
                GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition(gameData[currentGameIndex].SceneName);
                //SceneManager.LoadScene(gameData[currentGameIndex].SceneName);
            }
            */
        }

        void ShakePopup()
        {
            AudioManager.I.PlaySfx(Sfx.UIPopup);
            Popup.GetComponent<RectTransform>().DOPunchScale(new Vector2(0.2f, 0.2f), 1.0f, 5, 1f)
                .SetLoops(0).SetUpdate(true).SetAutoKill(true)
                .SetEase(Ease.InOutQuad).Play();
        }

        public void OnWheelStart()
        {
            TutorialArrow.SetActive(false);
            WidgetSubtitles.I.Close();
            AudioManager.I.PlaySfx(Sfx.WheelStart);

        }

        public void OnWheelStopped()
        {
            AudioManager.I.StopSfx(Sfx.WheelStart);
            GameIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>(gameData[currentGameIndex].GetIconResourcePath());
            AudioManager.I.PlayMusic(Music.Relax);
            isGameSelected = true;
            ShakePopup();

            switch (gameData[currentGameIndex].Code) {
                case "fastcrowd":
                    WidgetSubtitles.I.DisplaySentence("wheel_game_fastcrowd", 2, true);
                    break;
                case "fastcrowd_words":
                    WidgetSubtitles.I.DisplaySentence("wheel_game_fastcrowdword", 2, true);
                    break;
                case "dontwakeup":
                    WidgetSubtitles.I.DisplaySentence("wheel_game_dontwake", 2, true);
                    break;
                case "balloons":
                    WidgetSubtitles.I.DisplaySentence("wheel_game_balloons_end", 2, true);
                    break;
            }

            ContinueScreen.Show(OnPopuplicked, ContinueScreenMode.Button);
            // AudioManager.I.PlayMusic2();
            //GameIcon.SetActive(true);
        }

        public void OnRadiusTrigger(int number, Color _color)
        {
            

            if (WheelCntrl.isRotating) {
                if (number != currentGameIndex) {
                    currentGameIndex = (number % numberOfGames);
                    //Debug.Log("OnRadiusTrigger" + currentSector);

                    PopupImage.color = _color;

                    showGameIcon(currentGameIndex);
                    AudioManager.I.PlaySfx(Sfx.WheelTick);
                    //AudioManager.I.PlayHit();
                }
            }

            // if isQuiteStopped and is correct game perform a super breck to select this game.
            if (WheelCntrl.isQuiteStopped && currentGameIndex == gameIndexToForceSelect)
                WheelCntrl.IsBrakeForceEnhanced = true;

        }

        void showGameIcon(int index)
        {
            if (index >= 0) {
                GameIcon.SetActive(true);
                labelText.text = ArabicFixer.Fix(gameData[index].Title, false, false);
                GameIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>(gameData[index].GetIconResourcePath());
            } else {
                labelText.text = "";
                GameIcon.SetActive(false);
            }
        }
    }
}