using UnityEngine;
using System;
using EA4S.MinigamesAPI;
using EA4S.MinigamesCommon;

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

        public bool CheatMode = false;

        private bool _ignoreJourneyData = false;
        public bool IgnoreJourneyData {
            get { return _ignoreJourneyData; }
            set {
                _ignoreJourneyData = value;
                Teacher.ConfigAI.forceJourneyIgnore = _ignoreJourneyData;
            }
        }

        private DifficultyLevel _difficultyLevel = DifficultyLevel.Normal;
        public DifficultyLevel DifficultyLevel {
            get { return _difficultyLevel; }
            set {
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

        public float Difficulty = 0.5f;
        public int Stage = 1;
        public int LearningBlock = 1;
        public int PlaySession = 1;

        void Awake()
        {
            I = this;
        }

        void Update()
        {
            // refactor: these are not assigned to anything at all!

            if (Input.GetKeyDown(KeyCode.Space)) {
                UnityEngine.Debug.Log("DEBUG - SPACE : skip");
            }

            if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0)) {
                UnityEngine.Debug.Log("DEBUG - 0");
            }

            if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) {
                UnityEngine.Debug.Log("DEBUG - 1");
            }

            if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) {
                UnityEngine.Debug.Log("DEBUG - 2");
            }

            if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3)) {
                UnityEngine.Debug.Log("DEBUG - 3");
            }
        }

        // refactor: this should be merged with MiniGameAPI and MiniGameLauncher
        public void LaunchMinigGame(MiniGameCode miniGameCodeSelected)
        {
            UnityEngine.Debug.Log("LaunchMinigGame " + miniGameCodeSelected.ToString());
            AppManager.I.Player.CurrentJourneyPosition.Stage = Stage;
            AppManager.I.Player.CurrentJourneyPosition.LearningBlock = LearningBlock;
            AppManager.I.Player.CurrentJourneyPosition.PlaySession = PlaySession;

            // We must force this or the teacher won't use the correct data
            AppManager.I.Teacher.InitialiseCurrentPlaySession();

            // Call start game with parameters
            MiniGameAPI.Instance.StartGame(
                miniGameCodeSelected,
                new GameConfiguration(Difficulty)
            );

        }

    }
}
