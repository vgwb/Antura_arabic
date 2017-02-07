using System;
﻿using EA4S.Database;
using System.Collections.Generic;
using EA4S.Environment;
using EA4S.Rewards;
using EA4S.Teacher;
using UnityEngine;
using EA4S.Profile;

namespace EA4S.Core
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
        Assessment,
        AnturaSpace,
        Rewards,
        PlaySessionResult, 
        DebugPanel
    }

    public struct NavigationData
    {
        public PlayerProfile CurrentPlayer;
        public AppScene PrevScene;
        public AppScene CurrentScene;
        public bool RealPlaySession;

        /// <summary>
        /// List of minigames selected for the current play session
        /// </summary>
        public List<MiniGameData> CurrentPlaySessionMiniGames;

        /// <summary>
        /// Current minigame index in
        /// </summary>
        public int CurrentMiniGameIndexInPlaySession { get; private set; }

        public void SetFirstMinigame()
        {
            CurrentMiniGameIndexInPlaySession = 0;
        }

        public bool SetNextMinigame()
        {
            int NextIndex = CurrentMiniGameIndexInPlaySession + 1;
            if (NextIndex < CurrentPlaySessionMiniGames.Count)
            {
                CurrentMiniGameIndexInPlaySession = NextIndex;
                return true;
            }
            return false;
        }

        public MiniGameData CurrentMiniGameData
        {
            get
            {
                if (CurrentPlaySessionMiniGames == null) return null;
                if (CurrentPlaySessionMiniGames.Count == 0) return null;
                return CurrentPlaySessionMiniGames[CurrentMiniGameIndexInPlaySession];
            }
        }
    }

    /// <summary>
    /// Controls the transitions between different scenes in the application.
    /// </summary>
    public class NavigationManager : MonoBehaviour
    {
        public NavigationData NavData;

        public bool IsLoadingMinigame { get; private set; } // Daniele mod - SceneTransitioner needs it to know when a minigame is being loaded

        #region API

        /// <summary>
        /// Sets the player navigation data.
        /// </summary>
        /// <param name="_playerProfile">The player profile.</param>
        public void SetPlayerNavigationData(PlayerProfile _playerProfile) {
            NavData.CurrentPlayer = _playerProfile ;
        }
        #endregion

        #region Automatic navigation API

        /// <summary>
        /// Given the current context, selects the scene that should be loaded next and loads it.
        /// </summary>
        // refactor: the whole NavigationManager could work using just GoToNextScene (and similars, such as GoBack), so that it controls all scene movement
        public void GoToNextScene()
        {
            Debug.LogFormat(" ---- NAV MANAGER ({1}) scene {0} ---- ", NavData.CurrentScene, "GoToNextScene");
            //var nextScene = GetNextScene();
            switch (NavData.CurrentScene)
            {
                case AppScene.Home:

                    if (NavData.CurrentPlayer.IsFirstContact()) {
                        GoToScene(AppScene.Intro);
                    } else {
                        if (NavData.CurrentPlayer.MoodAlreadyAnswered) {
                            GoToScene(AppScene.Map);
                        } else {
                            GoToScene(AppScene.Mood);
                        }
                    }

                    break;
                case AppScene.Mood:
                    GoToScene(AppScene.Map);
                    break;
                case AppScene.Map:
                    GotoPlaysessione();
                    break;
                case AppScene.Book:
                    break;
                case AppScene.Intro:
                    GoToScene(AppScene.Map);
                    break;
                case AppScene.GameSelector:
                    GotoFirsGameOfPlaysession();
                    break;
                case AppScene.MiniGame:
                    GotoNextGameOfPlaysession();
                    break;
                case AppScene.AnturaSpace:
                    GoToScene(AppScene.Map);
                    break;
                case AppScene.Rewards:
                    if (NavData.CurrentPlayer.IsFirstContact()) {
                        GoToScene(AppScene.AnturaSpace);
                    } else {
                        MaxJourneyPositionProgress();
                        GoToScene(AppScene.Map);
                    }
                    break;
                case AppScene.PlaySessionResult:
                    GoToScene(AppScene.Map);
                    break;
                case AppScene.DebugPanel:
                    NavData.SetFirstMinigame();
                    InternalLaunchGameScene(NavData.CurrentMiniGameData);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Apply logic for back button in current scene.
        /// </summary>
        public void GoBack() {
            Debug.LogFormat(" ---- NAV MANAGER ({1}) scene {0} ---- ", NavData.CurrentScene, "GoBack");
            switch (NavData.CurrentScene) {
                case AppScene.Book:
                case AppScene.GameSelector:
                case AppScene.AnturaSpace:
                    GoToScene(AppScene.Map);
                    break;
                default:
                    GoToScene(NavData.PrevScene);
                    break;
            }
        }

        #endregion

        #region Direct navigation (private)

        // refactor: GoToScene can be separated for safety reasons in GoToMinigame (with a string code, or minigame code) and GoToAppScene (with an AppScene as the parameter)
        private void GoToScene(string sceneName)
        {
            IsLoadingMinigame = sceneName.Substring(0, 5) == "game_";
            // TODO: change scenemodule to private for this class
            Debug.LogFormat(" ==== {0} scene to load ====", sceneName);
            AppManager.Instance.Modules.SceneModule.LoadSceneWithTransition(sceneName, new ModularFramework.Modules.SceneTransition() { });

            if (AppConstants.UseUnityAnalytics) {
                UnityEngine.Analytics.Analytics.CustomEvent("changeScene", new Dictionary<string, object> { { "scene", sceneName } });
            }
        }

        private void GoToScene(AppScene newScene)
        {
            // Additional checks for specific scenes
            switch (newScene) {
                case AppScene.Rewards:
                    // Already rewarded this playsession?
                    if (RewardSystemManager.RewardAlreadyUnlocked(NavData.CurrentPlayer.CurrentJourneyPosition)) {
                        GoToScene(AppScene.Map);
                        return;
                    }
                    break;
                default:
                    // Do nothing
                    break;
            }

            // Scene switch
            NavData.PrevScene = NavData.CurrentScene;
            NavData.CurrentScene = newScene;

            var nextSceneName = GetSceneName(newScene);
            GoToScene(nextSceneName);
        }

        /// <summary>
        /// Launches the game scene.
        /// !!! WARNING !!! Direct call is allowed only by NavigationManager internal, Book and debugger.
        /// </summary>
        /// <param name="_miniGame">The mini game.</param>
        private void InternalLaunchGameScene(MiniGameData _miniGame)
        {
            AppManager.I.GameLauncher.LaunchGame(_miniGame.Code);
        }

        #endregion

        #region Special request

        /// <summary>
        /// Go to home if is allowed for current scene.
        /// </summary>
        public void GoToHome()
        {
            Debug.LogFormat(" ---- NAV MANAGER ({1}) scene {0} ---- ", NavData.CurrentScene, "GoToHome");
            switch (NavData.CurrentScene) {
                case AppScene.DebugPanel:
                    GoToScene(AppScene.Home);
                    break;
                default:
                    break;
            }
        }

        public void GoToPlayerBook()
        {
            Debug.LogFormat(" ---- NAV MANAGER ({1}) scene {0} ---- ", NavData.CurrentScene, "GoToPlaybook");
            GoToScene(AppScene.Book);
        }

        public void GoToAnturaSpace() {
            Debug.LogFormat(" ---- NAV MANAGER ({1}) scene {0} ---- ", NavData.CurrentScene, "GoToAnturaSpace");
            // no restrictions?
            if (NavData.CurrentPlayer.IsFirstContact())
                GoToScene(AppScene.Rewards);
            else
                GoToScene(AppScene.AnturaSpace);
        }

        public void ExitAndGoHome()
        {
            Debug.LogFormat(" ---- NAV MANAGER ({1}) scene {0} ---- ", NavData.CurrentScene, "ExitAndGoHome");
            if (NavData.CurrentScene == AppScene.Map)
            {
                GoToScene(AppScene.Home);
            }
            else {
                GoToScene(AppScene.Map);
            }
        }

        public void GotoMinigameScene()
        {
            bool canTravel = false;

            switch (NavData.CurrentScene) {

                // Normal flow
                case AppScene.MiniGame:
                case AppScene.GameSelector:
                case AppScene.Map:
                    canTravel = true;
                    break;

                // "Fake minigame" flow
                default:
                    canTravel = !NavData.RealPlaySession;
                    break;
            }

            if (canTravel)
            {
                NavData.PrevScene = NavData.CurrentScene;
                NavData.CurrentScene = AppScene.MiniGame;
                GoToScene(NavData.CurrentMiniGameData.Scene);
            }
            else
            {
                throw new Exception("Cannot go to a minigame from the current scene!");
            }

        }

        // obsolete: to be implemented?
        public void ExitCurrentGame() { }

        #endregion

        // refactor: scene names should match AppScene so that this can be removed
        public string GetSceneName(AppScene scene, Database.MiniGameData minigameData = null)
        {
            switch (scene) {
                case AppScene.Home:
                    return "_Start";
                case AppScene.Mood:
                    return "app_Mood";
                case AppScene.Map:
                    return "app_Map";
                case AppScene.Book:
                    return "app_Book";
                case AppScene.Intro:
                    return "app_Intro";
                case AppScene.GameSelector:
                    return "app_GamesSelector";
                case AppScene.MiniGame:
                    return minigameData.Scene;
                case AppScene.Assessment:
                    return "app_Assessment";
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

        // refactor: move this method directly to PlayerProfile
        public void MaxJourneyPositionProgress()
        {
            AppManager.I.Player.SetMaxJourneyPosition(TeacherAI.I.journeyHelper.FindNextJourneyPosition(AppManager.I.Player.CurrentJourneyPosition));
        }

        // obsolete: unused
        /*public string GetCurrentScene()
        {
            return "";
        }*/

        // refactor: move these a more coherent manager, which handles the state of a play session between minigames
        #region temp for demo
        List<EndsessionResultData> EndSessionResults = new List<EndsessionResultData>();

        /// <summary>
        /// Called to notify end minigame with result (pushed continue button on UI).
        /// </summary>
        /// <param name="_stars">The stars.</param>
        public void EndMinigame(int _stars)
        {
            if (NavData.CurrentMiniGameData == null)
                return;
            EndsessionResultData res = new EndsessionResultData(_stars, NavData.CurrentMiniGameData.GetIconResourcePath(), NavData.CurrentMiniGameData.GetBadgeIconResourcePath());
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
        /// Calculates the unlock item count in accord to gameplay result information.
        /// </summary>
        /// <returns></returns>
        public int CalculateUnlockItemCount()
        {
            int totalEarnedStars = 0;
            for (int i = 0; i < EndSessionResults.Count; i++) {
                totalEarnedStars += EndSessionResults[i].Stars;
            }
            // Add bones to player
            int unlockItemsCount = 0;
            if (EndSessionResults.Count > 0) {
                float starRatio = totalEarnedStars / EndSessionResults.Count;
                // Prevent aproximation errors (0.99f must be == 1 but 0.7f must be == 0)
                unlockItemsCount = starRatio - Mathf.CeilToInt(starRatio) < 0.0001f
                    ? Mathf.CeilToInt(starRatio)
                    : Mathf.RoundToInt(starRatio);
            }
            // decrement because the number of stars needed to unlock the first reward is 2.
            unlockItemsCount -= 1;
            return unlockItemsCount;
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


        #region Michele

        public MiniGameData CurrentMiniGameData
        {
            get { return NavData.CurrentMiniGameData; }
        }

        public List<MiniGameData> CurrentPlaySessionMiniGames
        {
            get { return NavData.CurrentPlaySessionMiniGames; }
        }

        public void InitialiseNewPlaySession(MiniGameData dataToUse = null)
        {
            NavData.RealPlaySession = (dataToUse == null);

            AppManager.I.Teacher.InitialiseNewPlaySession();
            NavData.SetFirstMinigame();

            if (NavData.RealPlaySession)
            {
                NavData.CurrentPlaySessionMiniGames = AppManager.I.Teacher.SelectMiniGames();
            }
            else
            {
                NavData.CurrentPlaySessionMiniGames = new List<MiniGameData>();
                NavData.CurrentPlaySessionMiniGames.Add(dataToUse);
            }
        }

        private void GotoPlaysessione()
        {
            // This must be called before any play session is started
            InitialiseNewPlaySession();

            // From the map
            if (AppManager.I.Teacher.journeyHelper.IsAssessmentTime(NavData.CurrentPlayer.CurrentJourneyPosition))
            {
                // Direct to the current minigame (which is an assessment)
                InternalLaunchGameScene(NavData.CurrentMiniGameData);
            }
            else
            {
                // Show the games selector
                GoToScene(AppScene.GameSelector);
            }
        }

        private void GotoFirsGameOfPlaysession()
        {
            // Game selector -> go to the first game
            NavData.SetFirstMinigame();
            // TODO: ???
            WorldManager.I.CurrentWorld = (WorldID)(NavData.CurrentPlayer.CurrentJourneyPosition.Stage - 1);
            InternalLaunchGameScene(NavData.CurrentMiniGameData);
        }

        private void GotoNextGameOfPlaysession()
        {
            // From one game to the next
            if (AppManager.I.Teacher.journeyHelper.IsAssessmentTime(NavData.CurrentPlayer.CurrentJourneyPosition))
            {
                // Assessment ended, go to the rewards scene
                GoToScene(AppScene.Rewards);
            }
            else
            {
                // Not an assessment. Do we have any more?
                if (NavData.SetNextMinigame())
                {
                    // Go to the next minigame.
                    InternalLaunchGameScene(NavData.CurrentMiniGameData);
                }
                else
                {
                    // Finished all minigames for the current play session
                    if (NavData.RealPlaySession)
                    {
                        // Go to the reward scene.
                        GoToScene(AppScene.PlaySessionResult);
                    }
                    else
                    {
                        // Go where you were previously
                        GoBack();
                    }
                }
            }
        }


        #endregion
    }
}
