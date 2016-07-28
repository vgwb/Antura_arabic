using UnityEngine;
using System.Collections.Generic;
using EA4S;
using ModularFramework.Core;
using ModularFramework.Helpers;
using Google2u;
using System;
using ModularFramework.Modules;

namespace EA4S.DontWakeUp
{
    public enum How2Die {
        Null,
        TouchedDog,
        TouchedAlarm,
        TooFast,
        TooSlow,
        Fall
    }

    public class GameDontWakeUp : MiniGameBase
    {
        [Header("Gameplay Info and Config section")]
        #region Overrides
        new public GameDontWakeUpGameplayInfo GameplayInfo;

        new public static GameDontWakeUp Instance
        {
            get { return SubGame.Instance as GameDontWakeUp; }
        }

        #endregion

        [Header("Don't Wake Up vars")]
        public MinigameState currentState;
        public int LivesLeft;

        public int currentRound;
        int currentLevel;
        LevelController currentLevelController;
        public GameObject[] Levels;
        public DangerMeter dangering;
        float dangeringSpeed = 1.0f;
        bool inDanger;
        float dangerIntensity;
        How2Die dangerCause;

        public GameObject myLetter;
        public GameObject StarSystems;
        public WordFlexibleContainer LivesContainer;

        public WordData currentWord;

        int RoundsTotal;

        void Start() {
            currentState = MinigameState.Initializing;
            RoundsTotal = Levels.Length;
            currentRound = 1;
            currentLevel = 1;
            LivesLeft = 3;
            AppManager.Instance.InitDataAI();

            LoggerEA4S.Log("minigame", "dontwakeup", "start", "");
            LoggerEA4S.Save();

            AudioManager.I.PlayMusic(Music.Lullaby);
            AudioManager.I.PlaySound("Dog/Snoring");

            GameIntro();
        }

        public void CloseScene() {
            StopSceneSounds();
        }

        void StopSceneSounds() {
            AudioManager.I.StopSfx(Sfx.DangerClock);
            AudioManager.I.StopSound("Dog/Snoring");
        }

        void GameIntro() {
            currentState = MinigameState.GameIntro;
            WidgetSubtitles.I.DisplaySentence("game_dontwake_intro1");
            WidgetNextButton.I.Show(ClickedNext);
        }

        public void GameIntroFinished() {
            WidgetSubtitles.I.DisplaySentence("");
            InitRound();
        }

        public void ClickedNext() {
            Debug.Log("ClickedNext()");
            switch (currentState) {
                case MinigameState.RoundIntro:
                    WidgetSubtitles.I.DisplaySentence("");
                    currentState = MinigameState.Playing;
                    break;
                case MinigameState.GameIntro:
                    GameIntroFinished();
                    break;
                case MinigameState.RoundEnd:
                    currentLevelController.DoAlarmOff();
                    WidgetSubtitles.I.DisplaySentence("");
                    InitRound();
                    break;
            }
        }

        public void InitRound() {
            currentState = MinigameState.RoundIntro;

            UpdateLivesContainer();
            SetupLevel();

            myLetter.SetActive(true);
            myLetter.GetComponent<MyLetter>().Init(currentWord.Key);
            myLetter.transform.position = currentLevelController.GetStartPosition().position;
            AudioManager.I.PlayWord(currentWord.Key);

            WidgetPopupWindow.I.Init(ClickedNext, "Carefully drag this word", currentWord.Key, currentWord.Word);
            WidgetSubtitles.I.DisplaySentence("init round");
        }

        void SetupLevel() {
            currentLevelController = Levels[currentLevel - 1].GetComponent<LevelController>();
            currentWord = AppManager.Instance.Teacher.GimmeAGoodWordData();
            // Debug.Log("word chosen: " + currentWord._id);
            LoggerEA4S.Log("minigame", "dontwakeup", "newWord", currentWord.Word);

            currentLevelController.SetWord();
            ChangeCamera(false);
        }

        public void RoundLost(How2Die how) {
            if (currentState != MinigameState.RoundEnd) {
                currentState = MinigameState.RoundEnd;
                resetDanger();
                myLetter.SetActive(false);

                switch (how) {
                    case How2Die.TouchedAlarm:
                        currentLevelController.DoAlarmEverything();
                        WidgetSubtitles.I.DisplaySentence("do not touch the alarms!");
                        break;
                    case How2Die.TouchedDog:
                        WidgetSubtitles.I.DisplaySentence("do not touch Antura!");
                        break;
                    case How2Die.TooFast:
                        WidgetSubtitles.I.DisplaySentence("don't go too fast!");
                        break;
                    case How2Die.Fall:
                        WidgetSubtitles.I.DisplaySentence("don't fall!");
                        break;
                }

                RoundLostEnded();
            }
        }

        void RoundLostEnded() {
            AudioManager.I.StopSfx(Sfx.DangerClock);
            AudioManager.I.PlaySfx(Sfx.Lose);

            if (LivesLeft > 1) {
                LivesLeft = LivesLeft - 1;

                //WidgetSubtitles.I.DisplaySentence("game_result_retry");
                WidgetNextButton.I.Show(ClickedNext);
            } else {
                GameLost();
            }
        }

        public void RoundWon() {
            currentState = MinigameState.Paused;
            myLetter.SetActive(false);
            if (currentRound < RoundsTotal) {
                GoToNextRound();
            } else {
                GameWon();
            }
        }

        public void GameWon() {
            currentState = MinigameState.GameEnd;
            StopSceneSounds();

            AudioManager.I.PlaySfx(Sfx.Win);
            StarSystems.SetActive(true);
            StarSystems.GetComponent<StarFlowers>().Show(3);
            LoggerEA4S.Log("minigame", "dontwakeup", "Won", "");
            LoggerEA4S.Save();
        }

        public void GameLost() {
            currentState = MinigameState.GameEnd;
            StopSceneSounds();

            StarSystems.SetActive(true);
            StarSystems.GetComponent<StarFlowers>().Show(0);
            LoggerEA4S.Log("minigame", "dontwakeup", "Lost", "");
            LoggerEA4S.Save();
        }

        void GoToNextRound() {
            LoggerEA4S.Log("minigame", "dontwakeup", "wordFinished", "");
            Levels[currentLevel - 1].SetActive(false);
            currentRound = currentRound + 1;
            currentLevel = currentRound;
            Levels[currentLevel - 1].SetActive(true);
            currentLevelController = Levels[currentLevel - 1].GetComponent<LevelController>();
            ChangeCamera(true);
        }
            
        // called by callback in camera
        public void CameraReady() {
            InitRound();
        }

        public void ChangeCamera(bool animated) {
            if (animated) {
                CameraGameplayController.I.MoveToPosition(currentLevelController.LevelCamera.transform.position, currentLevelController.LevelCamera.transform.rotation);
            } else {
                CameraGameplayController.I.SetToPosition(currentLevelController.LevelCamera.transform.position, currentLevelController.LevelCamera.transform.rotation);
            }
        }


        void UpdateLivesContainer() {
            LivesContainer.SetText(LivesLeft.ToString(), false);
        }



        void Update() {
//            if (Input.GetMouseButtonDown(0)) {
//                ClickedAnything();
//            }
            if (GameDontWakeUp.Instance.currentState == MinigameState.Playing) {
                if (inDanger) {
                    if (dangerCause == How2Die.TooFast) {
                        // toofast danger speed in faster!
                        dangerIntensity = dangerIntensity + dangeringSpeed * 1 * Time.deltaTime;
                    } else {
                        dangerIntensity = dangerIntensity + dangeringSpeed * Time.deltaTime;
                    }

                    if (dangerIntensity > 1f) {
                        dangerIntensity = 1f;
                        RoundLost(dangerCause);
                    }

                } else {
                    dangerIntensity = dangerIntensity - dangeringSpeed * Time.deltaTime;
                    if (dangerIntensity < 0)
                        dangerIntensity = 0;
                }

                dangering.SetIntensity(dangerIntensity);
            }
        }

        public void InDanger(bool status, How2Die cause) {
            inDanger = status;
            dangerCause = cause;
            if (inDanger) {
                AudioManager.I.PlaySfx(Sfx.DangerClock);
            } else {
                AudioManager.I.StopSfx(Sfx.DangerClock);
            }
        }

        void resetDanger() {
            InDanger(false, How2Die.Null);
            dangerIntensity = 0;
        }

    }

    [Serializable]
    public class GameDontWakeUpGameplayInfo : AnturaGameplayInfo
    {
        [Tooltip("Play session duration in seconds.")]
        public float PlayTime = 10;
    }


}