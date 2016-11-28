using UnityEngine;
using ModularFramework.Core;
using System.Collections;
using EA4S.Db;
using System.Collections.Generic;

namespace EA4S
{
    public enum AppScene
    {
        Home,
        Mood,
        Map,
        Book,
        Intro,
        GameSelector,
        MiniGame,
        AnturaSpace,
        Rewards,
        PlaySessionResult,
        DebugPanel
    }

    public class NavigationManager : MonoBehaviour
    {
        public static NavigationManager I;

        public AppScene CurrentScene;

        void Start()
        {
            I = this;
        }

        public void GoToScene(AppScene nextScene)
        {
            var nextSceneName = GetSceneName(nextScene);
            GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition(nextSceneName);
        }

        public void GoToGameScene(MiniGameData _miniGame)
        {
            //GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition(GetSceneName(AppScene.MiniGame, _miniGame));
            AppManager.I.GameLauncher.LaunchGame(_miniGame.Code);
        }

        //public string GetNextScene()
        //{
        //    return "";
        //}

        public void GoToNextScene()
        {
            //var nextScene = GetNextScene();
            switch (CurrentScene) {
                case AppScene.Home:
                    break;
                case AppScene.Mood:
                    break;
                case AppScene.Map:
                    if (AppManager.I.IsAssessmentTime)
                        GoToGameScene(TeacherAI.I.CurrentMiniGame);
                    else
                        GoToScene(AppScene.GameSelector);
                    break;
                case AppScene.Book:
                    break;
                case AppScene.Intro:
                    break;
                case AppScene.GameSelector:
                    AppManager.I.Player.ResetPlaySessionMinigame();
                    GoToGameScene(TeacherAI.I.CurrentMiniGame);
                    break;
                case AppScene.MiniGame:
                    if (AppManager.I.IsAssessmentTime) {
                        // TODO: never called
                        // assessment ended!
                        AppManager.I.Player.ResetPlaySessionMinigame();
                        GoToScene(AppScene.Rewards);
                    } else {
                        AppManager.I.Player.NextPlaySessionMinigame();
                        if (AppManager.I.Player.CurrentMiniGameInPlaySession >= TeacherAI.I.CurrentPlaySessionMiniGames.Count) {
                            /// - Update Journey
                            /// - Reset CurrentMiniGameInPlaySession
                            /// - Reward screen
                            /// *-- check first contact : 
                            AppManager.I.Player.SetMaxJourneyPosition(TeacherAI.I.journeyHelper.FindNextJourneyPosition(AppManager.I.Player.CurrentJourneyPosition));
                            AppManager.I.Player.ResetPlaySessionMinigame();
                            GoToScene(AppScene.PlaySessionResult);
                        } else {
                            // Next game
                            GoToGameScene(TeacherAI.I.CurrentMiniGame);
                        }
                    }
                    break;
                case AppScene.AnturaSpace:
                    break;
                case AppScene.Rewards:
                    //AppManager.I.Player.SetMaxJourneyPosition(TeacherAI.I.journeyHelper.FindNextJourneyPosition(AppManager.I.Player.CurrentJourneyPosition));
                    GoToScene(AppScene.Map);
                    break;
                case AppScene.PlaySessionResult:
                    GoToScene(AppScene.Map);
                    break;
                case AppScene.DebugPanel:
                    GoToGameScene(AppManager.I.CurrentMinigame);
                    break;
                default:
                    break;
            }
        }

        public string GetCurrentScene()
        {
            return "";
        }



        public void GoHome()
        {

        }

        public void GoBack() { }

        public void ExitCurrentGame() { }

        public void OpenPlayerBook()
        {
            GoToScene(AppScene.Book);
        }

        public void ExitAndGoHome()
        {
            if (CurrentScene == AppScene.Map) {
                GoToScene(AppScene.Home);
            } else {
                GoToScene(AppScene.Map);
            }
        }

        public string GetSceneName(AppScene scene, Db.MiniGameData minigameData = null)
        {
            switch (scene) {
                case AppScene.Home:
                    return "_Start";
                case AppScene.Mood:
                    return "app_Mood";
                case AppScene.Map:
                    return "app_Map";
                case AppScene.Book:
                    return "app_PlayerBook";
                case AppScene.Intro:
                    return "app_Intro";
                case AppScene.GameSelector:
                    return "app_GamesSelector";
                case AppScene.MiniGame:
                    return minigameData.Scene;
                case AppScene.AnturaSpace:
                    return "app_AnturaSpace";
                case AppScene.Rewards:
                    return "app_Rewards";
                case AppScene.PlaySessionResult:
                    return "app_PlaySessionResult";
                default:
                    return "";
            }
        }

        #region temp for demo
        List<EndsessionResultData> EndSessionResults = new List<EndsessionResultData>();

        /// <summary>
        /// Called to notify end minigame with result (pushed continue button on UI).
        /// </summary>
        /// <param name="_stars">The stars.</param>
        public void EndMinigame(int _stars)
        {
            if (TeacherAI.I.CurrentMiniGame == null)
                return;
            EndsessionResultData res = new EndsessionResultData(_stars, TeacherAI.I.CurrentMiniGame.GetIconResourcePath(), TeacherAI.I.CurrentMiniGame.GetBadgeIconResourcePath());
            EndSessionResults.Add(res);

        }

        /// <summary>
        /// Uses the end session results and reset it.
        /// </summary>
        /// <returns></returns>
        public List<EndsessionResultData> UseEndSessionResults()
        {
            List<EndsessionResultData> returnResult = EndSessionResults;
            EndSessionResults = new List<EndsessionResultData>();
            return returnResult;
        }

        /// <summary>
        /// Calculates the unlock item count.
        /// </summary>
        /// <returns></returns>
        public int CalculateUnlockItemCount()
        {
            // TODO: logic to calculate
            return 2;
        }

        /// <summary>
        /// Called to notify end of playsession (pushed continue button on UI).
        /// </summary>
        /// <param name="_stars">The star.</param>
        /// <param name="_bones">The bones.</param>
        public void EndPlaySession(int _stars, int _bones)
        {
            // Logic
            // log
            // GoToScene ...
        }
        #endregion
    }
}