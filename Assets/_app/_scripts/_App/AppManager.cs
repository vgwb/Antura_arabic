using UnityEngine;
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
    /// Works as a general manager and entry point for all other systems and managers.
    /// </summary>
    public class AppManager : Singleton<AppManager>
    {
        public AppSettings AppSettings = new AppSettings();

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
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

        #region Modules DEPRECATED

        [HideInInspector]
        public ModuleManager Modules = new ModuleManager();

        public PlayerProfileModule PlayerProfile
        {
            get { return Modules.PlayerProfile; }
        }

        public LocalizationModule Localization
        {
            get { return Modules.LocalizationModule; }
        }

        #endregion

        private PlayerProfileManager _playerProfileManager;

        /// <summary>
        /// Gets or sets the player profile manager.
        /// Reload GameSettings at any playerProfileManager changes.
        /// </summary>
        /// <value>
        /// The player profile manager.
        /// </value>
        public PlayerProfileManager PlayerProfileManager
        {
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

        public Profile.PlayerProfile Player
        {
            get { return PlayerProfileManager != null ? PlayerProfileManager.CurrentPlayer : null; }
            set { PlayerProfileManager.CurrentPlayer = value; }
        }

        #region Initialisation

        /// <summary>
        /// Prevent multiple setups.
        /// Set to true after first setup.
        /// </summary>
        bool alreadySetup = false;

        /// <summary>
        /// Game entry point.
        /// </summary>
        protected override void Initialise()
        {
            if (alreadySetup)
                return;

            base.Initialise();

            // Modules Setup
            Modules.ModulesSetup();
            Modules.ModuleAutoInstallerOverride(this.gameObject);

            alreadySetup = true;

            // Debugger setup
            Debug.logger.logEnabled = AppConstants.VerboseLogging;
#if SRDebuggerEnabled
            if (AppConstants.DebugPanelEnabled) {
                SRDebug.Init();
            }
#endif

            DB = new DatabaseManager(AppSettings.UseTestDatabase);
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

            // Update settings
            AppSettings.ApplicationVersion = AppConstants.AppVersion;
            PlayerProfileManager.SaveGameSettings();
        }

        #endregion


        void Update()
        {
            // Exit with Android back button
            if (Input.GetKeyDown(KeyCode.Escape)) {
                if (Application.platform == RuntimePlatform.Android) {
                    GlobalUI.ShowPrompt(Database.LocalizationDataId.UI_AreYouSure, () => {
                        Debug.Log("Application Quit");
                        Application.Quit();
                    }, () => { });
                }
            }

            // shortcut to Reserved Area
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R)) {
                NavigationManager.GoToReservedArea();
            }
        }

        #region Settings behaviours

        // refactor: should be moved to AppManager
        public void ToggleQualitygfx()
        {
            AppSettings.HighQualityGfx = !AppSettings.HighQualityGfx;
            CameraGameplayController.I.EnableFX(AppSettings.HighQualityGfx);
        }

        public void ToggleEnglishSubtitles()
        {
            AppSettings.EnglishSubtitles = !AppSettings.EnglishSubtitles;
            PlayerProfileManager.SaveGameSettings();
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

        public void OpenSupportForm()
        {
            var parameters = "";
            parameters += "?entry.346861357=" + WWW.EscapeURL(JsonUtility.ToJson(new DeviceInfo()));
            parameters += "&entry.1999287882=" + WWW.EscapeURL(JsonUtility.ToJson(Player));

            Application.OpenURL(AppConstants.UrlSupportForm + parameters);
        }

        #region TMPro hack
        /// <summary>
        /// TextMesh Pro hack to manage Diacritic Symbols correct positioning
        /// </summary>
        void OnEnable()
        {
            // Subscribe to event fired when text object has been regenerated.
            TMPro.TMPro_EventManager.TEXT_CHANGED_EVENT.Add(On_TMPro_Text_Changed);
        }

        void OnDisable()
        {
            TMPro.TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(On_TMPro_Text_Changed);
        }

        void On_TMPro_Text_Changed(Object obj)
        {
            TMPro.TMP_Text _tmp_text = obj as TMPro.TMP_Text;
            if (_tmp_text != null && VocabularyHelper.FixDiacriticPositions(_tmp_text.textInfo)) {
                _tmp_text.UpdateVertexData();
            }
        }
        #endregion
    }
}