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

        int tutorialIndex = 10;

        void Start()
        {
            AudioManager.I.PlayMusic(SceneMusic);
            ShowProgression();
            Debug.Log("MapManager PlaySession " + AppManager.Instance.Player.CurrentJourneyPosition.PlaySession);
            if ((AppManager.Instance.Player.CurrentJourneyPosition.PlaySession) == 1) {
                tutorialIndex = 10;
            } else if ((AppManager.Instance.Player.CurrentJourneyPosition.PlaySession) == 2) {
                tutorialIndex = 20;
            } else if ((AppManager.Instance.Player.CurrentJourneyPosition.PlaySession) > 2) {
                tutorialIndex = 30;
            }

            SceneTransitioner.Close();

            ShowTutor();
        }

        public void ShowTutor()
        {
            switch (tutorialIndex) {
                case 10:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence("map_A1", 2, true, ShowTutor);
                    break;
                case 11:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence("map_A2", 2, true, ShowTutor);
                    break;
                case 12:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence("map_A3", 3, true, ShowTutor);
                    break;
                case 13:
                    WidgetSubtitles.I.DisplaySentence("map_A4", 2, true);
                    Zoom();
                    break;
                case 20:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence("map1_A1", 2, true, ShowTutor);
                    break;
                case 21:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence("map1_A2", 2, true);
                    Zoom();
                    break;
                case 30:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence("assessment_intro_A1", 2, true, ShowTutor);
                    break;
                case 31:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence("assessment_intro_A2", 2, true, ShowTutor);
                    break;
                case 32:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence("assessment_intro_A3", 2, true, ShowTutor);
                    break;
                case 33:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence("assessment_intro_A4", 2, true);
                    Zoom();
                    break;
            }
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
                GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_Assessment");
            else
                GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Wheel");
        }

        void ShowProgression()
        {
            CurrentSteps[0].SetActive(AppManager.Instance.Player.CurrentJourneyPosition.PlaySession > 0);
            CurrentSteps[1].SetActive(AppManager.Instance.Player.CurrentJourneyPosition.PlaySession > 1);
            CurrentSteps[2].SetActive(AppManager.Instance.Player.CurrentJourneyPosition.PlaySession > 2);
            CurrentSteps[3].SetActive(AppManager.Instance.Player.CurrentJourneyPosition.PlaySession > 3);
            CurrentSteps[4].SetActive(AppManager.Instance.Player.CurrentJourneyPosition.PlaySession > 4);

            Vector3 currentDotPosition = CurrentSteps[AppManager.Instance.Player.CurrentJourneyPosition.PlaySession].transform.position;

            Player.transform.position = new Vector3(currentDotPosition.x, currentDotPosition.y + 4.6f, currentDotPosition.z);

        }

        public void ChangeCamera()
        {

            CameraGameplayController.I.MoveToPosition(ZoomCameraGO.transform.position, ZoomCameraGO.transform.rotation);

        }

        private class PlaySessionState
        {
            public Db.PlaySessionData data;
            public float score;

            public PlaySessionState(Db.PlaySessionData _data, float _score)
            {
                this.data = _data;
                this.score = _score;
            }
        }

        /// <summary>
        /// Returns a list of all play session data with its score (if a score exists) for the given stage
        /// </summary>
        /// <param name="_stage"></param>
        /// <returns></returns>
        private List<PlaySessionState> GetAllPlaySessionStateForStage(int _stage)
        {
            // Get all available scores for this stage
            List<Db.ScoreData> scoreData_list = AppManager.Instance.Teacher.GetCurrentScoreForPlaySessionsOfStage(_stage);

            // For each score entry, get its play session data and build a structure containing both
            List<PlaySessionState> playSessionState_list = new List<PlaySessionState>();
            for (int i = 0; i < scoreData_list.Count; i++) {
                var data = AppManager.Instance.DB.GetPlaySessionDataById(scoreData_list[i].ElementId);
                playSessionState_list.Add(new PlaySessionState(data, scoreData_list[i].Score));
            }

            return playSessionState_list;
        }

        /// <summary>
        /// Given a stage, returns the list of all play session data corresponding to it.
        /// </summary>
        /// <param name="_stage"></param>
        /// <returns></returns>
        private List<Db.PlaySessionData> GetAllPlaySessionDataForStage(int _stage)
        {
            return AppManager.Instance.DB.FindPlaySessionData(x => x.Stage == _stage);
        }

    }

}