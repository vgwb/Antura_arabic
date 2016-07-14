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
        MinigameState currentState;
        int currentRound;
        LevelController currentLevelController;
        public GameObject[] Levels;
        public DangerMeter dangering;
        public GameObject StarSystems;
        public GameObject Subtitles;
        public GameObject PopupWindow;

        public wordsRow currentWord;

        void Start() {
            currentState = MinigameState.Initializing;
            currentRound = 1;
            AppManager.Instance.InitDataAI();

            Logger.Log("minigame", "fastcrowd", "start", GameplayInfo.PlayTime.ToString());
            Logger.Save();

            SetupLevel();

            // GameplayTimer.Instance.StartTimer(GameplayInfo.PlayTime);

            AudioManager.I.PlayMusic(Music.Relax);
        }

        public void SetupLevel() {
            currentLevelController = Levels[currentRound - 1].GetComponent<LevelController>();
 
            currentWord = AppManager.Instance.Teacher.GimmeAGoodWord();
            Debug.Log("word chosen: " + currentWord._id);

            currentLevelController.SetWord();


            AudioManager.I.PlayWord(currentWord._id);

        }

        public void Won() {
            
            StarSystems.SetActive(true);
        }

        public void FinishedLevel(int quale) {
            if (quale != currentRound) {
                ChangeCamera();
            }
        }

        public void ChangeCamera() {
            currentRound = (currentRound + 1) % 3;
            currentLevelController = Levels[currentRound - 1].GetComponent<LevelController>();
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