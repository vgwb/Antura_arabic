using Antura.Core;
using Antura.Database;
using Antura.Minigames;
using Antura.Profile;
using Antura.Rewards;
using UnityEngine;

namespace Antura.Debugging
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
        public bool TutorialEnabled = false;

        #endregion

        #region App Options

        public bool VerboseTeacher {
            get { return Teacher.ConfigAI.verboseTeacher; }
            set { Teacher.ConfigAI.verboseTeacher = value; }
        }

        /// <summary>
        /// Stops a MiniGame from playing if the PlaySession database does not allow that MiniGame to be played at a given position.
        /// </summary>
        public bool SafeLaunch = true;

        /// <summary>
        /// If SafeLaunch is on, the DebugManager will correct the journey position so the minimum JP is selected
        /// </summary>
        public bool AutoCorrectJourneyPos = true;

        private bool _ignoreJourneyData = false;

        public bool IgnoreJourneyData {
            get { return _ignoreJourneyData; }
            set {
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
        public bool FirstContactPassed {
            get { return !AppManager.I.Player.IsFirstContact(); }
            set {
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
                // RESERVED AREA
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R)) {
                    AppManager.I.NavigationManager.GoToReservedArea();
                }

                // ADD BONES
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.B)) {
                    AddBones();
                }

                if (Input.GetKeyDown(KeyCode.Space)) {
                    Debug.Log("DEBUG - SPACE : skip");
                    if (OnSkipCurrentScene != null) OnSkipCurrentScene();
                }

                if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0)) {
                    ForceCurrentMinigameEnd(0);
                }

                if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) {
                    ForceCurrentMinigameEnd(1);
                }

                if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) {
                    ForceCurrentMinigameEnd(2);
                }

                if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3)) {
                    ForceCurrentMinigameEnd(3);
                }
            }
        }

        #endregion

        #region Actions

        public void CreateTestProfile()
        {
            AppManager.I.PlayerProfileManager.CreatePlayerProfile(4, PlayerGender.F, 1, PlayerTint.Blue);
            AppManager.I.NavigationManager.GoToHome(debugMode: true);
        }

        public void AddBones()
        {
            AppManager.I.Player.AddBones(10);
        }

        public void ForceCurrentMinigameEnd(int stars)
        {
            if (OnForceCurrentMinigameEnd != null) {
                Debug.Log("DEBUG - Force Current Minigame End with stars: " + stars);
                OnForceCurrentMinigameEnd(stars);
            }
        }

        public void EnableDebugPanel()
        {
            DebugPanelEnabled = true;
            if (debugPanelGO == null) {
                debugPanelGO = Instantiate(Resources.Load("Prefabs/Debug/UI Debug Canvas") as GameObject);
            }
        }

        public void LaunchMiniGame(MiniGameCode miniGameCodeSelected, float difficulty)
        {
            AppManager.I.Player.CurrentJourneyPosition.SetPosition(Stage, LearningBlock, PlaySession);

            Difficulty = difficulty;

            Debug.Log("LaunchMiniGame " + miniGameCodeSelected + " PS: " + AppManager.I.Player.CurrentJourneyPosition + " Diff: " +
                      Difficulty + " Tutorial: " + TutorialEnabled);
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
            if (newPos != null) {
                AppManager.I.Player.SetMaxJourneyPosition(newPos, true);
            }
        }

        public void SecondToLastJourneyPos()
        {
            JourneyPosition newPos = AppManager.I.JourneyHelper.GetFinalJourneyPosition();
            newPos.PlaySession = 2;
            if (newPos != null) {
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
            foreach (RewardPackUnlockData pack in RewardSystemManager.GetNextRewardPack()) {
                AppManager.I.Player.AddRewardUnlocked(pack);
                Debug.LogFormat("Pack added: {0}", pack.ToString());
            }
            JourneyPosition next = AppManager.I.JourneyHelper.FindNextJourneyPosition(AppManager.I.Player.CurrentJourneyPosition);
            if (next != null) {
                AppManager.I.Player.SetMaxJourneyPosition(new JourneyPosition(next.Stage, next.LearningBlock, next.PlaySession));
                AppManager.I.Player.SetCurrentJourneyPosition(new JourneyPosition(next.Stage, next.LearningBlock, next.PlaySession));
            }
        }

        public void UnlockAllRewards()
        {
            RewardSystemManager.UnlockAllRewards();
        }

        #endregion
    }
}