using UnityEngine;
using EA4S.Core;
using EA4S.Database;
using EA4S.MinigamesAPI;
using EA4S.Rewards;

namespace EA4S.Debugging
{

    /// <summary>
    /// General manager for debug purposes.
    /// Allows debugging via forcing the value of some parameters of the application.
    /// </summary>
    public class DebugManager : MonoBehaviour
    {
        public static DebugManager I;

        public bool DebugPanelEnabled;
        public bool DebugPanelOpened;

        public delegate void OnSkipCurrentSceneDelegate();
        public static event OnSkipCurrentSceneDelegate OnSkipCurrentScene;
        public delegate void OnForceCurrentMinigameEndDelegate(int value);
        public static event OnForceCurrentMinigameEndDelegate OnForceCurrentMinigameEnd;

        private GameObject debugPanelGO;

        #region Launch Options

        public int Stage = 1;
        public int LearningBlock = 1;
        public int PlaySession = 1;

        public void SetDebugJourneyPos(JourneyPosition jp)
        {
            Stage = jp.Stage;
            LearningBlock = jp.LearningBlock;
            PlaySession = jp.PlaySession;
        }

        public float Difficulty = 0.5f;
        public int NumberOfRounds = 1;
        public bool TutorialEnabled = true;

        #endregion

        #region App Options

        public bool VerboseTeacher { get { return Teacher.ConfigAI.verboseTeacher; } set { Teacher.ConfigAI.verboseTeacher = value; } }

        public bool CheatEnabled = false;

        /// <summary>
        /// Stops a MiniGame from playing if the PlaySession database does not allow that MiniGame to be played at a given position.
        /// </summary>
        public bool SafeLaunch = true;

        /// <summary>
        /// If SafeLaunch is on, the DebugManager will correct the journey position so the minimum JP is selected
        /// </summary>
        public bool AutoCorrectJourneyPos = true;

        private bool _ignoreJourneyData = false;
        public bool IgnoreJourneyData
        {
            get { return _ignoreJourneyData; }
            set
            {
                _ignoreJourneyData = value;
                Teacher.ConfigAI.forceJourneyIgnore = _ignoreJourneyData;
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether [first contact passed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [first contact passed]; otherwise, <c>false</c>.
        /// </value>
        public bool FirstContactPassed
        {
            get { return !AppManager.I.Player.IsFirstContact(); }
            set
            {
                if (value) {
                    AppManager.I.Player.FirstContactPassed(2);
                } else {
                    AppManager.I.Player.ResetPlayerProfileCompletion();
                }
            }
        }


        #region Unity events

        void Awake()
        {
            I = this;

            if (AppConstants.DebugPanelEnabledAtStartup) {
                EnableDebugPanel();
            }
        }

        void Update()
        {
            if (!DebugPanelOpened) {
                // shortcut to Reserved Area
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R)) {
                    AppManager.I.NavigationManager.GoToReservedArea();
                }

                if (Input.GetKeyDown(KeyCode.Space)) {
                    Debug.Log("DEBUG - SPACE : skip");
                    if (OnSkipCurrentScene != null) OnSkipCurrentScene();
                }

                if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0)) {
                    Debug.Log("DEBUG - 0");
                    if (OnForceCurrentMinigameEnd != null) OnForceCurrentMinigameEnd(0);
                }

                if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) {
                    Debug.Log("DEBUG - 1");
                    if (OnForceCurrentMinigameEnd != null) OnForceCurrentMinigameEnd(1);
                }

                if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) {
                    Debug.Log("DEBUG - 2");
                    if (OnForceCurrentMinigameEnd != null) OnForceCurrentMinigameEnd(2);
                }

                if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3)) {
                    Debug.Log("DEBUG - 3");
                    if (OnForceCurrentMinigameEnd != null) OnForceCurrentMinigameEnd(3);
                }
            }
        }

        #endregion

        #region Actions

        public void EnableDebugPanel()
        {
            DebugPanelEnabled = true;
            if (debugPanelGO == null) {
                debugPanelGO = Instantiate(Resources.Load("Prefabs/Debug/UI Debug Canvas") as GameObject);
            }
        }

        #region Launch

        public void LaunchMiniGame(MiniGameCode miniGameCodeSelected, float difficulty)
        {
            AppManager.I.Player.CurrentJourneyPosition.Stage = Stage;
            AppManager.I.Player.CurrentJourneyPosition.LearningBlock = LearningBlock;
            AppManager.I.Player.CurrentJourneyPosition.PlaySession = PlaySession;
            AppManager.I.Player.CurrentJourneyPosition.SetPosition(Stage, LearningBlock, PlaySession);

            Difficulty = difficulty;

            Debug.Log("LaunchMiniGame " + miniGameCodeSelected + " PS: " + AppManager.I.Player.CurrentJourneyPosition + " Diff: " + Difficulty + " Tutorial: " + TutorialEnabled);
            AppManager.I.GameLauncher.LaunchGame(miniGameCodeSelected,
                new MinigameLaunchConfiguration(Difficulty, NumberOfRounds, tutorialEnabled: TutorialEnabled), forceNewPlaySession: true);
        }

        public void ResetAll()
        {
            AppManager.I.PlayerProfileManager.ResetEverything();
            AppManager.I.NavigationManager.GoToHome(debugMode: true);
            Debug.Log("Reset ALL players and DB.");
        }

        #endregion

        #region Navigation

        public void GoToHome()
        {
            AppManager.I.NavigationManager.GoToHome(debugMode: true);
        }

        public void GoToMap()
        {
            AppManager.I.NavigationManager.GoToMap(debugMode: true);
        }

        public void GoToNext()
        {
            AppManager.I.NavigationManager.GoToNextScene();
        }

        public void GoToEnd()
        {
            AppManager.I.NavigationManager.GoToEnding(debugMode: true);
        }

        public void GoToReservedArea()
        {
            AppManager.I.NavigationManager.GoToReservedArea(debugMode: true);
        }

        public void ForwardMaxJourneyPos()
        {
            JourneyPosition newPos = AppManager.I.JourneyHelper.FindNextJourneyPosition(AppManager.I.Player.MaxJourneyPosition);
            if (newPos != null)
            {
                AppManager.I.Player.SetMaxJourneyPosition(newPos, true);
            }
        }

        public void SecondToLastJourneyPos()
        {
            JourneyPosition newPos = AppManager.I.JourneyHelper.GetFinalJourneyPosition();
            newPos.PlaySession = 2;
            if (newPos != null)
            {
                AppManager.I.Player.SetMaxJourneyPosition(newPos, true);
                FirstContactPassed = true;
            }
            GoToMap();
        }

        public void ResetMaxJourneyPos()
        {
            AppManager.I.Player.ResetMaxJourneyPosition();
        }

        #endregion

        #region Profiles

        public void CreateTestProfile()
        {
            AppManager.I.PlayerProfileManager.CreatePlayerProfile(4, PlayerGender.F, 1, PlayerTint.Blue);
            AppManager.I.NavigationManager.GoToHome(debugMode: true);
        }
        
        #region Rewards

        public void UnlockAll()
        {
            AppManager.I.Player.SetMaxJourneyPosition(new JourneyPosition(6, 15, 100), true);
        }

        public void UnlockFirstReward()
        {
            RewardSystemManager.UnlockFirstSetOfRewards();
        }

        public void UnlockNextPlaySessionRewards()
        {
            //JourneyPosition CurrentJourney = AppManager.I.Player.CurrentJourneyPosition;
            foreach (RewardPackUnlockData pack in RewardSystemManager.GetNextRewardPack())
            {
                AppManager.I.Player.AddRewardUnlocked(pack);
                Debug.LogFormat("Pack added: {0}", pack.ToString());
            }
            JourneyPosition next = AppManager.I.JourneyHelper.FindNextJourneyPosition(AppManager.I.Player.CurrentJourneyPosition);
            if (next != null)
            {
                AppManager.I.Player.SetMaxJourneyPosition(new JourneyPosition(next.Stage, next.LearningBlock, next.PlaySession));
                AppManager.I.Player.SetCurrentJourneyPosition(new JourneyPosition(next.Stage, next.LearningBlock, next.PlaySession));
            }
        }

        public void UnlockAllRewards()
        {
            RewardSystemManager.UnlockAllRewards();
        }

        #endregion

        #endregion

        #endregion

    }
}