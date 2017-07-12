using Antura.Audio;
using Antura.CameraControl;
using Antura.Core;
using Antura.Database;
using Antura.LivingLetters;
using Antura.Profile;
using Antura.Rewards;
using Antura.Teacher;
using Antura.UI;
using Antura.Utilities;
using UnityEngine;

namespace Antura
{
    /// <summary>
    /// Core of the application.
    /// Works as a general manager and entry point for all other systems and managers.
    /// </summary>
    public class AppManager : SingletonMonoBehaviour<AppManager>
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }

        public AppSettingsManager AppSettingsManager;
        public TeacherAI Teacher;
        public VocabularyHelper VocabularyHelper;
        public ScoreHelper ScoreHelper;
        public JourneyHelper JourneyHelper;
        public DatabaseManager DB;
        public MiniGameLauncher GameLauncher;
        public LogManager LogManager;

        [HideInInspector]
        public NavigationManager NavigationManager;

        public PlayerProfileManager PlayerProfileManager;

        public AppSettings AppSettings
        {
            get { return AppSettingsManager.Settings; }
        }

        public PlayerProfile Player
        {
            get { return PlayerProfileManager.CurrentPlayer; }
            set { PlayerProfileManager.CurrentPlayer = value; }
        }

        #region Initialisation

        /// <summary>
        /// Prevent multiple setups.
        /// Set to true after first setup.
        /// </summary>
        private bool alreadySetup;

        /// <summary>
        /// Game entry point.
        /// </summary>
        protected override void Init()
        {
            if (alreadySetup)
                return;

            base.Init();

            alreadySetup = true;

            AppSettingsManager = new AppSettingsManager();
            DB = new DatabaseManager();
            // TODO refactor: standardize initialisation of managers
            LogManager = new LogManager();
            VocabularyHelper = new VocabularyHelper(DB);
            JourneyHelper = new JourneyHelper(DB);
            ScoreHelper = new ScoreHelper(DB);
            Teacher = new TeacherAI(DB, VocabularyHelper, JourneyHelper, ScoreHelper);
            GameLauncher = new MiniGameLauncher(Teacher);

            NavigationManager = gameObject.AddComponent<NavigationManager>();
            NavigationManager.Init();

            PlayerProfileManager = new PlayerProfileManager();
            PlayerProfileManager.LoadSettings();

            gameObject.AddComponent<KeeperManager>();

            RewardSystemManager.Init();
            UIDirector.Init(); // Must be called after NavigationManager has been initialized

            // Debugger setup
            Debug.logger.logEnabled = AppConstants.DebugLogEnabled;
            gameObject.AddComponent<Debugging.DebugManager>();

            // Update settings
            AppSettings.ApplicationVersion = AppConstants.AppVersion;
            AppSettingsManager.SaveSettings();
        }

        #endregion

        void Update()
        {
            // Exit with Android back button
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    GlobalUI.ShowPrompt(Database.LocalizationDataId.UI_AreYouSure, () =>
                    {
                        Debug.Log("Application Quit");
                        Application.Quit();
                    }, () => { });
                }
            }
        }

        #region Setting

        public void ToggleQualitygfx()
        {
            AppSettings.HighQualityGfx = !AppSettings.HighQualityGfx;
            CameraGameplayController.I.EnableFX(AppSettings.HighQualityGfx);
        }

        public void ToggleEnglishSubtitles()
        {
            AppSettings.EnglishSubtitles = !AppSettings.EnglishSubtitles;
            AppSettingsManager.SaveSettings();
        }

        #endregion

        #region Pause

        public bool IsPaused { get; private set; }

        void OnApplicationPause(bool pauseStatus)
        {
            IsPaused = pauseStatus;

            // app is pausing
            if (IsPaused)
            {
                LogManager.I.LogInfo(InfoEvent.AppSuspend);
            }

            //app is resuming
            if (!IsPaused)
            {
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
            var tmpText = obj as TMPro.TMP_Text;
            if (tmpText != null && VocabularyHelper.FixDiacriticPositions(tmpText.textInfo))
            {
                tmpText.UpdateVertexData();
            }
        }

        #endregion
    }
}