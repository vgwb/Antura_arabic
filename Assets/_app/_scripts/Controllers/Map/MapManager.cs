using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EA4S;
using ModularFramework.Core;

namespace EA4S
{
    public class MapManager : MonoBehaviour
    {

        [Header("Scene Setup")]
        public Music SceneMusic;

        [Header("Journey")]
        public GameObject[] Pins;
        public GameObject[] CurrentSteps;

        [Header("References")]
        public GameObject Player;
        public GameObject ZoomCameraGO;

        void Start()
        {
            AudioManager.I.PlayMusic(SceneMusic);
            WidgetSubtitles.I.DisplaySentence("map_A1", 2, true, NextSentence);

            ShowProgression();
        }

        public void NextSentence()
        {
            WidgetSubtitles.I.DisplaySentence("map_A2", 3, true, NextSentence2);
        }

        public void NextSentence2()
        {
            WidgetSubtitles.I.DisplaySentence("map_A3", 3, true, Zoom);
        }

        public void Zoom()
        {
            ChangeCamera();
        }

        // called by callback in camera
        public void CameraReady()
        {
            Ready2Play();
        }

        public void Ready2Play()
        {
            ContinueScreen.Show(Play, ContinueScreenMode.Button);
        }

        public void Play()
        {
            if (AppManager.Instance.IsAssessmentTime)
                GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Assessment");
            else
                GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Wheel");
        }

        void ShowProgression()
        {
            CurrentSteps[0].SetActive(AppManager.Instance.PlaySession > 0);
            CurrentSteps[1].SetActive(AppManager.Instance.PlaySession > 0);
            CurrentSteps[2].SetActive(AppManager.Instance.PlaySession > 1);
            CurrentSteps[3].SetActive(AppManager.Instance.PlaySession > 2);
            CurrentSteps[4].SetActive(AppManager.Instance.PlaySession > 3);

            Vector3 currentDotPosition = CurrentSteps[AppManager.Instance.PlaySession].transform.position;

            Player.transform.position = new Vector3(currentDotPosition.x, currentDotPosition.y + 4.6f, currentDotPosition.z);
                
        }

        public void ChangeCamera()
        {

            CameraGameplayController.I.MoveToPosition(ZoomCameraGO.transform.position, ZoomCameraGO.transform.rotation);

        }
    }

}