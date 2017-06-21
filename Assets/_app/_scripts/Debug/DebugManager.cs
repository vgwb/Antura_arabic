using UnityEngine;
using System;
using EA4S.Core;
using EA4S.MinigamesAPI;

namespace EA4S.Debugging
{
    // refactor: this enum could be used throughout the application instead of just being in the DebugManager
    public enum DifficultyLevel
    {
        VeryEasy = 0,
        Easy = 1,
        Normal = 2,
        Hard = 3,
        VeryHard = 4
    }

    /// <summary>
    /// General manager for debug purposes.
    /// Allows debugging via forcing the value of some parameters of the application.
    /// </summary>
    public class DebugManager : MonoBehaviour
    {
        public static DebugManager I;

        public bool DebugPanelActivated;

        public delegate void OnSkipCurrentSceneDelegate();

        public static event OnSkipCurrentSceneDelegate OnSkipCurrentScene;

        public delegate void OnForceCurrentMinigameEndDelegate(int value);

        public static event OnForceCurrentMinigameEndDelegate OnForceCurrentMinigameEnd;

        public float Difficulty = 0.5f;
        public int Stage = 1;
        public int LearningBlock = 1;
        public int PlaySession = 1;
        public int NumberOfRounds = 1;


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

        private DifficultyLevel _difficultyLevel = DifficultyLevel.Normal;

        public DifficultyLevel DifficultyLevel
        {
            get { return _difficultyLevel; }
            set
            {
                _difficultyLevel = value;
                switch (_difficultyLevel) {
                    case DifficultyLevel.VeryEasy:
                        Difficulty = 0.1f;
                        break;
                    case DifficultyLevel.Easy:
                        Difficulty = 0.3f;
                        break;
                    case DifficultyLevel.Normal:
                        Difficulty = 0.5f;
                        break;
                    case DifficultyLevel.Hard:
                        Difficulty = 0.7f;
                        break;
                    case DifficultyLevel.VeryHard:
                        Difficulty = 1.0f;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

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
        }

        void Update()
        {
            if (!DebugPanelActivated) {
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

        public void LaunchMiniGame(MiniGameCode miniGameCodeSelected)
        {
            Debug.Log("LaunchMiniGame " + miniGameCodeSelected.ToString());
            AppManager.I.Player.CurrentJourneyPosition.Stage = Stage;
            AppManager.I.Player.CurrentJourneyPosition.LearningBlock = LearningBlock;
            AppManager.I.Player.CurrentJourneyPosition.PlaySession = PlaySession;

            AppManager.I.GameLauncher.LaunchGame(miniGameCodeSelected,
                new MinigameLaunchConfiguration(Difficulty, NumberOfRounds), forceNewPlaySession: true);
        }

        public void ResetAll()
        {
            AppManager.I.PlayerProfileManager.ResetEverything();
            AppManager.I.NavigationManager.GoToHome(debugMode: true);
            Debug.Log("Reset ALL players and DB.");
        }

        #endregion
    }
}