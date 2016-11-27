using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using ModularFramework.Core;
using ModularFramework.Helpers;
using EA4S;
using TMPro;

namespace EA4S.PianoWoof
{
    public class PianoWoofGameManager : MonoBehaviour
    {
        public StageTileController[] stageTiles;

        [Header("Game Parameters")] [Tooltip("in seconds")]
        public float lightsCycleDuration;

        private int CurrentTileIndex
        {
            get { return _currentTileIndex; }
            set
            {
                _currentTileIndex = (value % stageTilesCount + stageTilesCount) % stageTilesCount;
            }
        }

        private int PreviousTileIndex
        {
            get { return ((CurrentTileIndex - 1) % stageTilesCount + stageTilesCount) % stageTilesCount; }
        }

        private int _currentTileIndex;
        private int stageTilesCount;
        private float waitTime;


        void Start()
        {
            stageTilesCount = stageTiles.Length;
            ResetScene();
            CycleLights();
        }

        void Update()
        {
        
        }

        private void CycleLights()
        {
            StartCoroutine(CycleLights_Coroutine());
        }

        private void StopLights()
        {
            StopCoroutine(CycleLights_Coroutine());
        }


        private IEnumerator CycleLights_Coroutine()
        {
            while (true)
            {
                Debug.Log("Tiles: " + stageTilesCount + ", Current Index: " + CurrentTileIndex + ", Previous Index: " + PreviousTileIndex + ", Wait Time: " + waitTime);

                stageTiles[CurrentTileIndex].TurnLightOn();
                stageTiles[PreviousTileIndex].TurnLightOff();
                CurrentTileIndex++;

                yield return new WaitForSeconds(waitTime);
            }
        }

        private void StartNewRound()
        {
            ResetScene();
            BeginGameplay();

            //LoggerEA4S.Log("minigame", "PianoWoof", "start", - );
            //LoggerEA4S.Save();
        }

        private void ResetScene()
        {
            waitTime = lightsCycleDuration / stageTiles.Length;
            CurrentTileIndex = 0;

            for (int i = 0; i < stageTilesCount; i++)
            {
                stageTiles[i].TurnLightOff();
            }

            // To-do: Placeholder
            // Set Stage Letters
            stageTiles[0].SetLetter(AppManager.I.Teacher.GetAllTestLetterDataLL().GetRandomElement());
            stageTiles[1].SetLetter(AppManager.I.Teacher.GetAllTestLetterDataLL().GetRandomElement());
            stageTiles[2].SetLetter(AppManager.I.Teacher.GetAllTestLetterDataLL().GetRandomElement());
            stageTiles[3].SetLetter(AppManager.I.Teacher.GetAllTestLetterDataLL().GetRandomElement());
            //stageTiles[4].SetLetter(AppManager.I.Teacher.GetAllTestLetterDataLL().GetRandomElement());
            stageTiles[5].SetLetter(AppManager.I.Teacher.GetAllTestLetterDataLL().GetRandomElement());
            stageTiles[6].SetLetter(AppManager.I.Teacher.GetAllTestLetterDataLL().GetRandomElement());
            stageTiles[7].SetLetter(AppManager.I.Teacher.GetAllTestLetterDataLL().GetRandomElement());
        }

        private void BeginGameplay()
        {
        
        }
    }
}
