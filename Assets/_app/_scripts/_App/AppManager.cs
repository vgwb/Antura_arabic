using UnityEngine;
using ModularFramework.Core;
using ModularFramework.Modules;
using EA4S.Audio;
using EA4S.CameraControl;
using EA4S.Core;
using EA4S.Database;
using EA4S.Debugging;
using EA4S.Profile;
using EA4S.Rewards;
using EA4S.Teacher;
using EA4S.MinigamesAPI;
using EA4S.UI;

namespace EA4S
{
    /// <summary>
    /// Core of the application.
    /// Functions as a general manager and entry point for all other systems and managers.
    /// </summary>
    public class AppManager : GameManager
    {
        public new AppSettings GameSettings = new AppSettings();

        // refactor: AppManager.Instance should be the only entry point to the singleton
        public static AppManager I {
            get { return GameManager.Instance as AppManager; }
        }

        public TeacherAI Teacher;
        public VocabularyHelper VocabularyHelper;
        public ScoreHelper ScoreHelper;
        public JourneyHelper JourneyHelper;
        public DatabaseManager DB;
        public MiniGameLauncher GameLauncher;
        public LogManager LogManager;
        public NavigationManager NavigationManager;

        public bool IsPaused { get; private set; }

        private PlayerProfileManager _playerProfileManager;
        /// <summary>
        /// Gets or sets the player profile manager.
        /// Reload GameSettings at any playerProfileManager changes.
        /// </summary>
        /// <value>
        /// The player profile manager.
        /// </value>
        public PlayerProfileManager PlayerProfileManager {
            get { return _playerProfileManager; }
            set {
                if (_playerProfileManager != value) {
                    _playerProfileManager = value;
                    _playerProfileManager.ReloadGameSettings();
                    return;
                }
                _playerProfileManager = value;
            }
        }

        public Profile.PlayerProfile Player {
            get { return PlayerProfileManager != null ? PlayerProfileManager.CurrentPlayer : null; }
            set { PlayerProfileManager.CurrentPlayer = value; }
        }

        #region Initialisation

        /// <summary>
        /// Game entry point.
        /// </summary>
        protected override void GameSetup()
        {
            base.GameSetup();

            // Debugger setup
            Debug.logger.logEnabled = AppConstants.VerboseLogging;
            if (AppConstants.DebugPanelEnabled) {
                SRDebug.Init();
            }

            // GameplayModule
            if (GetComponentInChildren<ModuleInstaller<IGameplayModule>>()) {
                IGameplayModule moduleInstance = GetComponentInChildren<ModuleInstaller<IGameplayModule>>().InstallModule();
                Modules.GameplayModule.SetupModule(moduleInstance, moduleInstance.Settings);
            }

            DB = new DatabaseManager(GameSettings.UseTestDatabase);
            // refactor: standardize initialisation of managers
            LogManager = new LogManager();
            VocabularyHelper = new VocabularyHelper(DB);
            JourneyHelper = new JourneyHelper(DB);
            ScoreHelper = new ScoreHelper(DB);
            Teacher = new TeacherAI(DB, VocabularyHelper, JourneyHelper, ScoreHelper);
            GameLauncher = new MiniGameLauncher(Teacher);

            NavigationManager = gameObject.AddComponent<NavigationManager>();
            NavigationManager.Initialize();

            PlayerProfileManager = new PlayerProfileManager();
            gameObject.AddComponent<DebugManager>();
            gameObject.AddComponent<KeeperManager>();

            RewardSystemManager.Init();
            UIDirector.Init(); // Must be called after NavigationManager has been initialized

            GameSettings.HighQualityGfx = false;
        }

        #endregion

        #region Settings behaviours

        // refactor: should be moved to AppManager
        public void ToggleQualitygfx()
        {
            GameSettings.HighQualityGfx = !GameSettings.HighQualityGfx;
            CameraGameplayController.I.EnableFX(GameSettings.HighQualityGfx);
        }

        #endregion

        #region event delegate

        // obsolete: unused
        public void OnMinigameStart()
        {
        }

        #endregion

        #region Pause
        void OnApplicationPause(bool pauseStatus)
        {
            IsPaused = pauseStatus;

            // app is pausing
            if (IsPaused) {
                LogManager.I.LogInfo(InfoEvent.AppSuspend);
            }

            //app is resuming
            if (!IsPaused) {
                LogManager.I.LogInfo(InfoEvent.AppResume);
                LogManager.I.InitNewSession();
            }
            AudioManager.I.OnAppPause(IsPaused);
        }
        #endregion
    }
}