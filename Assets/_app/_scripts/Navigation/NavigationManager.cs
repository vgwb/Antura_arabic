using System;
using EA4S.Database;
using System.Collections.Generic;
using EA4S.Environment;
using EA4S.Rewards;
using UnityEngine;
using EA4S.Profile;

namespace EA4S.Core
{

    /// <summary>
    /// Controls the navigation among different scenes in the application.
    /// </summary>
    public class NavigationManager : MonoBehaviour
    {
        private static bool VERBOSE = false;

        public NavigationData NavData;

        public SceneTransitionManager SceneTransitionManager = new SceneTransitionManager();

        private List<KeyValuePair<AppScene, AppScene>> customTransitions = new List<KeyValuePair<AppScene, AppScene>>();
        private List<KeyValuePair<AppScene, AppScene>> backableTransitions = new List<KeyValuePair<AppScene, AppScene>>();

        #region State Checks

        public bool IsLoadingMinigame { get; private set; }

        public bool IsTransitioningScenes {
            get { return SceneTransitionManager.IsTransitioning; }
        }

        public Action OnSceneStartTransition {
            get { return SceneTransitionManager.OnSceneStartTransition; }
            set { SceneTransitionManager.OnSceneStartTransition = value; }
        }

        public Action OnSceneEndTransition {
            get { return SceneTransitionManager.OnSceneEndTransition; }
            set { SceneTransitionManager.OnSceneEndTransition = value; }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize the NavigationManager and its data.
        /// </summary>
        public void Initialize()
        {
            NavData.Setup();
            InitializeAllowedTransitions();
        }

        /// <summary>
        /// Initialize custom and 'back-enabled' transitions.
        /// </summary>
        private void InitializeAllowedTransitions()
        {
            // Allowed custom transitions
            customTransitions.Add(new KeyValuePair<AppScene, AppScene>(AppScene.Home, AppScene.PlayerCreation));
            customTransitions.Add(new KeyValuePair<AppScene, AppScene>(AppScene.Home, AppScene.ReservedArea));
            customTransitions.Add(new KeyValuePair<AppScene, AppScene>(AppScene.Map, AppScene.Book));
            customTransitions.Add(new KeyValuePair<AppScene, AppScene>(AppScene.Map, AppScene.AnturaSpace));
            customTransitions.Add(new KeyValuePair<AppScene, AppScene>(AppScene.Map, AppScene.Rewards));
            customTransitions.Add(new KeyValuePair<AppScene, AppScene>(AppScene.ReservedArea, AppScene.Book));

            // Transitions that can register for a 'back' function
            backableTransitions.Add(new KeyValuePair<AppScene, AppScene>(AppScene.Home, AppScene.ReservedArea));
            backableTransitions.Add(new KeyValuePair<AppScene, AppScene>(AppScene.Map, AppScene.Book));
            backableTransitions.Add(new KeyValuePair<AppScene, AppScene>(AppScene.Map, AppScene.AnturaSpace));
            backableTransitions.Add(new KeyValuePair<AppScene, AppScene>(AppScene.ReservedArea, AppScene.Book));
            backableTransitions.Add(new KeyValuePair<AppScene, AppScene>(AppScene.Map, AppScene.GameSelector));
        }

        /// <summary>
        /// Sets the player navigation data.
        /// </summary>
        /// <param name="_playerProfile">The player profile.</param>
        public void InitialisePlayerNavigationData(PlayerProfile _playerProfile)
        {
            NavData.Initialize(_playerProfile);
        }

        #endregion

        #region Automatic navigation API

        /// <summary>
        /// Given the current context, selects the scene that should be loaded next and loads it.
        /// This is related to the 'main' flow of the application.
        /// For 'custom' flows, refer to the custom route methods below.
        /// </summary>
        public void GoToNextScene()
        {
            if (VERBOSE) Debug.LogFormat(" ---- NAV MANAGER ({1}) scene {0} ---- ", NavData.CurrentScene, "GoToNextScene");
            switch (NavData.CurrentScene) {
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
                case AppScene.PlayerCreation:
                    GoToScene(AppScene.Intro);
                    break;
                case AppScene.Mood:
                    GoToScene(AppScene.Map);
                    break;
                case AppScene.Map:
                    GotoPlaysession();
                    break;
                case AppScene.Book:
                    break;
                case AppScene.Intro:
                    GoToScene(AppScene.Map);
                    break;
                case AppScene.GameSelector:
                    GotoFirstGameOfPlaySession();
                    break;
                case AppScene.MiniGame:
                    GotoNextGameOfPlaySession();
                    break;
                case AppScene.AnturaSpace:
                    GoToScene(AppScene.Map);
                    break;
                case AppScene.Rewards:
                    if (NavData.CurrentPlayer.IsFirstContact()) {
                        GoToScene(AppScene.AnturaSpace);
                    } else {
                        if (AppManager.I.Player.HasFinishedTheGame && !AppManager.I.Player.IsFinalShowed()) { 
                            AppManager.I.Player.SetFinalShowed();
                            GoToScene(AppScene.Ending);
                        } else {
                            AppManager.I.Player.AdvanceMaxJourneyPosition();
                            GoToScene(AppScene.Map);
                        }
                    }
                    break;
                case AppScene.PlaySessionResult:
                    GoToScene(AppScene.Map);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Apply logic for back button in current scene.
        /// </summary>
        public void GoBack()
        {
            /*Debug.LogError("HITTING BACK FROM " + NavData.CurrentScene);
            for (int i= 0; i<NavData.PrevSceneStack.Count; i++)
                Debug.LogError(i + ": " + NavData.PrevSceneStack.ToArray()[i]);
            */

            if (NavData.PrevSceneStack.Count > 0) {
                var prevScene = NavData.PrevSceneStack.Pop();
                if (VERBOSE) Debug.LogFormat(" ---- NAV MANAGER ({0}) from scene {1} to {2} ---- ", "GoBack", NavData.CurrentScene, prevScene);
                GoToScene(prevScene);
            }
        }

        #endregion

        #region Direct navigation (private)

        private void GoToScene(AppScene newScene, Database.MiniGameData minigameData = null)
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
            UpdatePrevSceneStack(newScene);
            NavData.CurrentScene = newScene;

            GoToSceneByName(AppSceneHelper.GetSceneName(newScene, minigameData));
        }

        private void GoToSceneByName(string sceneName)
        {
            IsLoadingMinigame = sceneName.Substring(0, 5) == "game_";

            if (VERBOSE) Debug.LogFormat(" ==== Loading scene {0} ====", sceneName);
            SceneTransitionManager.LoadSceneWithTransition(sceneName);

            if (AppConstants.UseUnityAnalytics && !Application.isEditor) {
                UnityEngine.Analytics.Analytics.CustomEvent("changeScene", new Dictionary<string, object> { { "scene", sceneName } });
            }
        }

        private void UpdatePrevSceneStack(AppScene newScene)
        {
            // The stack is updated only for some transitions
            if (backableTransitions.Contains(new KeyValuePair<AppScene, AppScene>(NavData.CurrentScene, newScene))) {
                if (NavData.PrevSceneStack.Count == 0 || NavData.PrevSceneStack.Peek() != NavData.CurrentScene) {
                    if (VERBOSE) Debug.LogError("Added BACKABLE transition " + NavData.CurrentScene + " to " + newScene);
                    NavData.PrevSceneStack.Push(NavData.CurrentScene);
                }
            }
        }

        /// <summary>
        /// Launches the game scene.
        /// </summary>
        /// <param name="_miniGame">The mini game.</param>
        private void InternalLaunchGameScene(MiniGameData _miniGame)
        {
            AppManager.I.GameLauncher.LaunchGame(_miniGame.Code);
        }

        #endregion

        #region Custom routes

        public void GoToHome(bool debugMode = false)
        {
            CustomGoTo(AppScene.Home, debugMode);
        }

        public void GoToPlayerBook()
        {
            CustomGoTo(AppScene.Book);
        }

        public bool PrevSceneIsReservedArea()
        {
            return NavData.PrevSceneStack.Peek() == AppScene.ReservedArea;
        }

        public void GoToPlayerCreation()
        {
            CustomGoTo(AppScene.PlayerCreation);
        }

        public void GoToReservedArea(bool debugMode = false)
        {
            CustomGoTo(AppScene.ReservedArea, debugMode);
        }

        public void GoToAnturaSpace()
        {
            switch (NavData.CurrentScene) {
                case AppScene.Map:
                    // First contact: we go to the Rewards scene instead
                    if (NavData.CurrentPlayer.IsFirstContact()) {
                        UpdatePrevSceneStack(AppScene.AnturaSpace); // We force the prev scene stack to hold the Map <-> AnturaSpace transition
                        CustomGoTo(AppScene.Rewards);
                    } else
                        CustomGoTo(AppScene.AnturaSpace);
                    break;
            }
        }

        /// <summary>
        /// Exit from the current scene. Called while in pause mode.
        /// </summary>
        public void ExitDuringPause()
        {
            Debug.LogFormat(" ---- NAV MANAGER ({1}) scene {0} ---- ", NavData.CurrentScene, "ExitDuringPause");
            switch (NavData.CurrentScene) {
                case AppScene.Map:
                case AppScene.PlayerCreation:
                    NavData.PrevSceneStack.Clear(); // We also clear the navigation data
                    GoToScene(AppScene.Home);
                    break;
                default:
                    GoToScene(AppScene.Map);
                    break;
            }
        }

        /// <summary>
        /// Internal GoTo to handle custom transitions.
        /// </summary>
        private void CustomGoTo(AppScene targetScene, bool debugMode = false)
        {
            if (debugMode || HasCustomTransitionTo(targetScene)) {
                Debug.LogFormat(" ---- NAV MANAGER ({0}) scene {1} to {2} ---- ", "CustomGoTo", NavData.CurrentScene, targetScene);
                GoToScene(targetScene);
            } else {
                throw new Exception("Cannot go to " + targetScene + " from " + NavData.CurrentScene);
            }
        }

        private bool HasCustomTransitionTo(AppScene targetScene)
        {
            return customTransitions.Contains(new KeyValuePair<AppScene, AppScene>(NavData.CurrentScene, targetScene));
        }

        /// <summary>
        /// Special GoTo for minigames.
        /// </summary>
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

            if (canTravel) {
                GoToScene(AppScene.MiniGame, NavData.CurrentMiniGameData);
            } else {
                throw new Exception("Cannot go to a minigame from the current scene!");
            }

        }

        #endregion

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


        #region Minigame Launching

        public MiniGameData CurrentMiniGameData {
            get { return NavData.CurrentMiniGameData; }
        }

        public List<MiniGameData> CurrentPlaySessionMiniGames {
            get { return NavData.CurrentPlaySessionMiniGames; }
        }

        public void InitialiseNewPlaySession(MiniGameData dataToUse = null)
        {
            NavData.RealPlaySession = (dataToUse == null);

            AppManager.I.Teacher.InitialiseNewPlaySession();
            NavData.SetFirstMinigame();

            if (NavData.RealPlaySession) {
                NavData.CurrentPlaySessionMiniGames = AppManager.I.Teacher.SelectMiniGames();
            } else {
                NavData.CurrentPlaySessionMiniGames = new List<MiniGameData>();
                NavData.CurrentPlaySessionMiniGames.Add(dataToUse);
            }
        }

        private void GotoPlaysession()
        {
            // This must be called before any play session is started
            InitialiseNewPlaySession();
            LogManager.I.StartPlaySession();

            // From the map
            if (AppManager.I.JourneyHelper.IsAssessmentTime(NavData.CurrentPlayer.CurrentJourneyPosition)) {
                // Direct to the current minigame (which is an assessment)
                InternalLaunchGameScene(NavData.CurrentMiniGameData);
            } else {
                // Show the games selector
                GoToScene(AppScene.GameSelector);
            }
        }

        private void GotoFirstGameOfPlaySession()
        {
            // Game selector -> go to the first game
            NavData.SetFirstMinigame();
            // TODO: ???
            WorldManager.I.CurrentWorld = (WorldID)(NavData.CurrentPlayer.CurrentJourneyPosition.Stage - 1);
            InternalLaunchGameScene(NavData.CurrentMiniGameData);
        }

        private void GotoNextGameOfPlaySession()
        {
            // From one game to the next
            if (AppManager.I.JourneyHelper.IsAssessmentTime(NavData.CurrentPlayer.CurrentJourneyPosition)) {
                // Assessment ended, go to the rewards scene
                GoToScene(AppScene.Rewards);
            } else {
                // Not an assessment. Do we have any more?
                if (NavData.SetNextMinigame()) {
                    // Go to the next minigame.
                    InternalLaunchGameScene(NavData.CurrentMiniGameData);
                } else {
                    // Finished all minigames for the current play session
                    if (NavData.RealPlaySession) {
                        // Go to the reward scene.
                        GoToScene(AppScene.PlaySessionResult);
                    } else {
                        // Go where you were previously
                        GoBack();
                    }
                }
            }
        }

        #endregion

        #region Scene Transition

        void OnEnable()
        {
            SceneTransitionManager.OnEnable();
        }

        void OnDisable()
        {
            SceneTransitionManager.OnDisable();
        }

        #endregion

    }
}
