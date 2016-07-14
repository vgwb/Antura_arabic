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
   
    public class GameDontWakeUp : MiniGameBase
    {
        [Header("Star Rewards")]
        public int ThresholdStar1 = 3;
        public int ThresholdStar2 = 6;
        public int ThresholdStar3 = 9;

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
        public GameObject myLetter;
        public GameObject StarSystems;
        public GameObject Subtitles;
        public GameObject PopupWindow;

        public wordsRow currentWord;

        void Start() {
            currentState = MinigameState.Initializing;
            currentRound = 1;
            AppManager.Instance.InitDataAI();

            LoggerEA4S.Log("minigame", "dontwakeup", "start", "");
            LoggerEA4S.Save();

            SetupLevel();

            AudioManager.I.PlayMusic(Music.Relax);
        }

        public void SetupLevel() {
            currentState = MinigameState.Popup;

            currentLevelController = Levels[currentRound - 1].GetComponent<LevelController>();
 
            currentWord = AppManager.Instance.Teacher.GimmeAGoodWord();
            Debug.Log("word chosen: " + currentWord._id);

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

        public void Won() {
            currentState = MinigameState.Ended;
            StarSystems.SetActive(true);
            StarSystems.GetComponent<StarFlowers>().Show(3);
        }

        public void Lost() {
            currentState = MinigameState.Ended;
            StarSystems.SetActive(true);
            StarSystems.GetComponent<StarFlowers>().Show(0);
        }

        public void FinishedLevel(bool success) {
            if (success) {
                currentState = MinigameState.Paused;
                myLetter.SetActive(false);
                if (currentRound < 3) {
                    Levels[currentRound - 1].SetActive(false);
                    currentRound = currentRound + 1;
                    Levels[currentRound - 1].SetActive(true);
                    currentLevelController = Levels[currentRound - 1].GetComponent<LevelController>();
                    ChangeCamera();
                } else {
                    Won();
                }
            } else {
                Lost();
                // ChangeCamera();
            }
        }

        // called by callback in camera
        public void CameraReady() {
            SetupLevel();
        }

        public void ChangeCamera() {
           
            CameraGameplayController.I.GoToPosition(currentLevelController.LevelCamera.transform.position, currentLevelController.LevelCamera.transform.rotation);
        }


    }

    [Serializable]
    public class GameDontWakeUpGameplayInfo : AnturaGameplayInfo
    {
        [Tooltip("Play session duration in seconds.")]
        public float PlayTime = 10;
    }


}