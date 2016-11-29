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

        int tutorialIndex = 10;

        void Start()
        {
            // Navigation manager 
            NavigationManager.I.CurrentScene = AppScene.Map;
            AudioManager.I.PlayMusic(SceneMusic);

            //ShowProgression();
            //Debug.Log("MapManager PlaySession " + AppManager.I.Player.CurrentJourneyPosition.PlaySession);
            //if ((AppManager.I.Player.CurrentJourneyPosition.PlaySession) == 1) {
            //    tutorialIndex = 10;
            //} else if ((AppManager.I.Player.CurrentJourneyPosition.PlaySession) == 2) {
            //    tutorialIndex = 20;
            //} else if ((AppManager.I.Player.CurrentJourneyPosition.PlaySession) > 2) {
            //    tutorialIndex = 30;
            //}


            //ShowTutor();

            //KeeperManager.I.PlayDialog(Db.LocalizationDataId.Map_Intro);
        }

        //public void ShowTutor()
        //{
        //    switch (tutorialIndex) {
        //        case 10:
        //            tutorialIndex++;
        //            WidgetSubtitles.I.DisplaySentence("map_A1", 2, true, ShowTutor);
        //            break;
        //        case 11:
        //            tutorialIndex++;
        //            WidgetSubtitles.I.DisplaySentence("map_A2", 2, true, ShowTutor);
        //            break;
        //        case 12:
        //            tutorialIndex++;
        //            WidgetSubtitles.I.DisplaySentence("map_A3", 3, true, ShowTutor);
        //            break;
        //        case 13:
        //            WidgetSubtitles.I.DisplaySentence("map_A4", 2, true);
        //            //Zoom();
        //            break;
        //        case 20:
        //            tutorialIndex++;
        //            WidgetSubtitles.I.DisplaySentence("map1_A1", 2, true, ShowTutor);
        //            break;
        //        case 21:
        //            tutorialIndex++;
        //            WidgetSubtitles.I.DisplaySentence("map1_A2", 2, true);
        //            //Zoom();
        //            break;
        //        case 30:
        //            tutorialIndex++;
        //            WidgetSubtitles.I.DisplaySentence("assessment_intro_A1", 2, true, ShowTutor);
        //            break;
        //        case 31:
        //            tutorialIndex++;
        //            WidgetSubtitles.I.DisplaySentence("assessment_intro_A2", 2, true, ShowTutor);
        //            break;
        //        case 32:
        //            tutorialIndex++;
        //            WidgetSubtitles.I.DisplaySentence("assessment_intro_A3", 2, true, ShowTutor);
        //            break;
        //        case 33:
        //            tutorialIndex++;
        //            WidgetSubtitles.I.DisplaySentence("assessment_intro_A4", 2, true);
        //            //Zoom();
        //            break;
        //    }
        //}

        //public void Zoom()
        //{
        //    ChangeCamera();
        //}

        //// called by callback in camera
        //public void CameraReady()
        //{
        //    Ready2Play();
        //}

        //public void Ready2Play()
        //{
        //    ContinueScreen.Show(Play, ContinueScreenMode.Button);
        //}

        //public void Play()
        //{
        //    AppManager.I.Teacher.InitialiseCurrentPlaySession();   // This must becalled before the games selector is loaded

        //    if (AppManager.I.IsAssessmentTime)
        //        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_Assessment");
        //    else
        //        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_GamesSelector");
        //}

        //void ShowProgression()
        //{
        //    CurrentSteps[0].SetActive(AppManager.I.Player.CurrentJourneyPosition.PlaySession > 0);
        //    CurrentSteps[1].SetActive(AppManager.I.Player.CurrentJourneyPosition.PlaySession > 1);
        //    CurrentSteps[2].SetActive(AppManager.I.Player.CurrentJourneyPosition.PlaySession > 2);
        //    CurrentSteps[3].SetActive(AppManager.I.Player.CurrentJourneyPosition.PlaySession > 3);
        //    CurrentSteps[4].SetActive(AppManager.I.Player.CurrentJourneyPosition.PlaySession > 4);

        //    Vector3 currentDotPosition = CurrentSteps[AppManager.I.Player.CurrentJourneyPosition.PlaySession].transform.position;

        //    Player.transform.position = new Vector3(currentDotPosition.x, currentDotPosition.y + 4.6f, currentDotPosition.z);

        //}

        //public void ChangeCamera()
        //{

        //    CameraGameplayController.I.MoveToPosition(ZoomCameraGO.transform.position, ZoomCameraGO.transform.rotation);

        //}


        //// TODO: use similar code to get info on the journey
        //private void TestGetJourneyInfo()
        //{
        //    int stage = 1;
        //    List<Teacher.LearningBlockInfo> learningBlockInfo_list = AppManager.I.Teacher.journeyHelper.GetLearningBlockInfosForStage(stage);
        //    foreach(var info in learningBlockInfo_list)
        //    {
        //        Debug.Log(info.data + ": " + info.score);
        //    }
        //}

    }

}