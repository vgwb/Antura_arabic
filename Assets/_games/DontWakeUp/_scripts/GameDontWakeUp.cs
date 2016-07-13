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
        [Header("Gameplay Info and Config section")]
        #region Overrides
        new public GameDontWakeUpGameplayInfo GameplayInfo;

        new public static GameDontWakeUp Instance
        {
            get { return SubGame.Instance as GameDontWakeUp; }
        }

        #endregion

        [Header("My vars")]
        int currentRound;
        public GameObject[] CameraPositions;
        public DangerMeter dangering;
        public GameObject StarSystems;
        public GameObject Subtitles;
        public GameObject PopupWindow;

        wordsRow currentWord;

        void Start() {
            AppManager.Instance.InitDataAI();
            currentWord = AppManager.Instance.Teacher.GimmeAGoodWord();
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
            CameraGameplayController.I.GoToPosition(CameraPositions[currentRound].transform.position, CameraPositions[currentRound].transform.rotation);
        }


    }

    [Serializable]
    public class GameDontWakeUpGameplayInfo : AnturaGameplayInfo
    {
        public float Time = 10;
    }


}