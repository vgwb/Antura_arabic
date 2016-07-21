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

        public wordsRow currentWord;

        int RoundsTotal;

        void Start() {
            currentState = MinigameState.Initializing;
            RoundsTotal = Levels.Length;
            currentRound = 1;
            LivesLeft = 3;
            UpdateLivesContainer();

            AppManager.Instance.InitDataAI();

            LoggerEA4S.Log("minigame", "dontwakeup", "start", "");
            LoggerEA4S.Save();

            SetupLevel();
            AudioManager.I.PlayMusic(Music.Relax);
        }

        public void SetupLevel() {
            currentState = MinigameState.RoundIntro;

            SubtitlesController.I.DisplayText("init round");

            currentLevelController = Levels[currentRound - 1].GetComponent<LevelController>();
 
            currentWord = AppManager.Instance.Teacher.GimmeAGoodWord();
            Debug.Log("word chosen: " + currentWord._id);
            LoggerEA4S.Log("minigame", "dontwakeup", "newWord", currentWord._word);

            currentLevelController.SetWord();
            myLetter.SetActive(true);
            myLetter.GetComponent<MyLetter>().Init(currentWord._id);
            myLetter.transform.position = currentLevelController.GetStartPosition().position;

            AudioManager.I.PlayWord(currentWord._id);
            AudioManager.I.PlaySound("Dog/Snoring");

            PopupWindow.SetActive(true);
            PopupWindow.GetComponent<PopupWindowController>().Init("Carefully drag this word", currentWord._id, currentWord._word);
        }

        public void PopupPressedContinue() {
            currentState = MinigameState.Playing;
        }

        public void RoundLost(How2Die how) {
            if (currentState != MinigameState.RoundEnd) {
                currentState = MinigameState.RoundEnd;

                AudioManager.I.PlaySfx(Sfx.Lose);

                switch (how) {
                    case How2Die.TouchedAlarm:
                        myLetter.SetActive(false);
                        currentLevelController.DoAlarmEverything();
                        AudioManager.I.StopSound("Dog/Snoring");
                        Invoke("RoundLostEnded", 2);
                        break;
                    default:
                        RoundLostEnded();
                        break;
                }
            }
        }

        void RoundLostEnded() {
            if (LivesLeft > 0) {
                SetupLevel();
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
            AudioManager.I.StopSfx(Sfx.DangerClock); // to-do: temporary fix?

            currentState = MinigameState.GameEnd;
            AudioManager.I.StopSound("Dog/Snoring");
            AudioManager.I.PlaySfx(Sfx.Win);
            StarSystems.SetActive(true);
            StarSystems.GetComponent<StarFlowers>().Show(3);
            LoggerEA4S.Log("minigame", "dontwakeup", "Won", "");
            LoggerEA4S.Save();
        }

        public void GameLost() {
            AudioManager.I.StopSfx(Sfx.DangerClock); // to-do: temporary fix?

            currentState = MinigameState.GameEnd;
            AudioManager.I.StopSound("Dog/Snoring");

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
            SetupLevel();
        }

        public void ChangeCamera() {
           
            CameraGameplayController.I.GoToPosition(currentLevelController.LevelCamera.transform.position, currentLevelController.LevelCamera.transform.rotation);
        }


        void UpdateLivesContainer() {
            LivesContainer.SetText(LivesLeft.ToString());
        }



        void Update() {
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

        public void InDanger(bool status) {
            inDanger = status;
            if (inDanger) {
                AudioManager.I.PlaySfx(Sfx.DangerClock);
            } else {
                AudioManager.I.StopSfx(Sfx.DangerClock);
            }
        }

    }

    [Serializable]
    public class GameDontWakeUpGameplayInfo : AnturaGameplayInfo
    {
        [Tooltip("Play session duration in seconds.")]
        public float PlayTime = 10;
    }


}