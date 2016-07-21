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
        TouchedDog,
        TouchedAlarm,
        TooFast,
        TooSlow,
        Fall
    }

    public class GameDontWakeUp : MiniGameBase
    {
        [Header("Star Rewards")]
        public int LivesLeft;

        [Header("Gameplay Info and Config section")]
        #region Overrides
        new public GameDontWakeUpGameplayInfo GameplayInfo;

        new public static GameDontWakeUp Instance
        {
            get { return SubGame.Instance as GameDontWakeUp; }
        }

        #endregion

        [Header("My vars")]
        public MinigameState currentState;
        public int currentRound;
        LevelController currentLevelController;
        public GameObject[] Levels;
        public DangerMeter dangering;
        float dangeringSpeed = 10f;
        bool inDanger;
        float dangerIntensity;

        public GameObject myLetter;
        public GameObject StarSystems;
        public GameObject Subtitles;
        public GameObject PopupWindow;
        public WordFlexibleContainer LivesContainer;
        public GameObject NextButtonGO;

        public WordData currentWord;

        int RoundsTotal;

        void Start() {
            currentState = MinigameState.Initializing;
            RoundsTotal = Levels.Length;
            currentRound = 1;
            LivesLeft = 3;
            AppManager.Instance.InitDataAI();

            LoggerEA4S.Log("minigame", "dontwakeup", "start", "");
            LoggerEA4S.Save();

            AudioManager.I.PlayMusic(Music.Relax);
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
            SubtitlesController.I.DisplaySentence("game_dontwake_intro1");
            NextButtonGO.SetActive(true);
        }

        public void GameIntroFinished() {
            SubtitlesController.I.DisplaySentence("");
            InitRound();
        }

        public void ClickedNext() {
            Debug.Log("ClickedNext()");
            switch (currentState) {
                case MinigameState.RoundIntro:
                    SubtitlesController.I.DisplaySentence("");
                    NextButtonGO.SetActive(false);
                    currentState = MinigameState.Playing;
                    break;
                case MinigameState.GameIntro:
                    GameIntroFinished();
                    break;
                case MinigameState.RoundEnd:
                    currentLevelController.DoAlarmOff();
                    SubtitlesController.I.DisplaySentence("");
                    NextButtonGO.SetActive(false);
                    InitRound();
                    break;
            }
        }

        public void InitRound() {
            currentState = MinigameState.RoundIntro;

            UpdateLivesContainer();

            currentLevelController = Levels[currentRound - 1].GetComponent<LevelController>();
            currentWord = AppManager.Instance.Teacher.GimmeAGoodWordData();
            // Debug.Log("word chosen: " + currentWord._id);
            LoggerEA4S.Log("minigame", "dontwakeup", "newWord", currentWord.Word);

            currentLevelController.SetWord();
            myLetter.SetActive(true);
            myLetter.GetComponent<MyLetter>().Init(currentWord.Key);
            myLetter.transform.position = currentLevelController.GetStartPosition().position;
            AudioManager.I.PlayWord(currentWord.Key);

            PopupWindow.SetActive(true);
            PopupWindow.GetComponent<PopupWindowController>().Init(ClickedNext, "Carefully drag this word", currentWord.Key, currentWord.Word);
            SubtitlesController.I.DisplaySentence("init round");
        }

        public void RoundLost(How2Die how) {
            if (currentState != MinigameState.RoundEnd) {
                currentState = MinigameState.RoundEnd;
                resetDanger();
                myLetter.SetActive(false);

                switch (how) {
                    case How2Die.TouchedAlarm:
                        currentLevelController.DoAlarmEverything();
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

                SubtitlesController.I.DisplaySentence("game_result_retry");
                NextButtonGO.SetActive(true);
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
            Levels[currentRound - 1].SetActive(false);
            currentRound = currentRound + 1;
            Levels[currentRound - 1].SetActive(true);
            currentLevelController = Levels[currentRound - 1].GetComponent<LevelController>();
            ChangeCamera();
        }
            
        // called by callback in camera
        public void CameraReady() {
            InitRound();
        }

        public void ChangeCamera() {
           
            CameraGameplayController.I.GoToPosition(currentLevelController.LevelCamera.transform.position, currentLevelController.LevelCamera.transform.rotation);
        }


        void UpdateLivesContainer() {
            LivesContainer.SetText(LivesLeft.ToString());
        }



        void Update() {
//            if (Input.GetMouseButtonDown(0)) {
//                ClickedAnything();
//            }
            if (GameDontWakeUp.Instance.currentState == MinigameState.Playing) {
                if (inDanger) {
                    dangerIntensity = dangerIntensity + dangeringSpeed * Time.deltaTime;
                    if (dangerIntensity > 1f) {
                        dangerIntensity = 1f;
                        RoundLost(How2Die.TouchedAlarm);
                    }
                } else {
                    dangerIntensity = dangerIntensity - dangeringSpeed * Time.deltaTime;
                    if (dangerIntensity < 0)
                        dangerIntensity = 0;
                }

                dangering.SetIntensity(dangerIntensity);
            }
        }

        public void InDanger(bool status) {
            inDanger = status;
            if (inDanger) {
                AudioManager.I.PlaySfx(Sfx.DangerClock);
            } else {
                AudioManager.I.StopSfx(Sfx.DangerClock);
            }
        }

        void resetDanger() {
            InDanger(false);
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