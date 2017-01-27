using UnityEngine;
using ModularFramework.Core;
using ModularFramework.Modules;
using EA4S.Audio;
using EA4S.CameraControl;
using EA4S.Core;
using EA4S.Database;
using EA4S.Debugging;
using EA4S.MinigamesAPI;
using EA4S.Profile;
using EA4S.Rewards;
using EA4S.Teacher;
using PlayerProfile = EA4S.Profile.PlayerProfile;

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
        public DatabaseManager DB;
        public PlayerProfile Player;
        public MiniGameLauncher GameLauncher;
        public LogManager LogManager;
        public PlayerProfileManager PlayerProfileManager;

        // refactor: access to the current minigame data should be in another subsystem that is responsible for holding temporary data for minigames
        [HideInInspector]
        public Database.MiniGameData CurrentMinigame;

        bool appIsPaused = false;

        #region Initialisation

        /// <summary>
        /// Game entry point.
        /// </summary>
        protected override void GameSetup()
        {
            base.GameSetup();

            if (AppConstants.DebugPanelEnabled) {
                SRDebug.Init();
            }

            // GameplayModule
            if (GetComponentInChildren<ModuleInstaller<IGameplayModule>>()) {
                IGameplayModule moduleInstance = GetComponentInChildren<ModuleInstaller<IGameplayModule>>().InstallModule();
                Modules.GameplayModule.SetupModule(moduleInstance, moduleInstance.Settings);
            }

            // refactor: standardize initialisation of managers
            gameObject.AddComponent<MiniGameAPI>();

            LogManager = new LogManager();
            PlayerProfileManager = new PlayerProfileManager();

            gameObject.AddComponent<DebugManager>();
            gameObject.AddComponent<NavigationManager>();
            gameObject.AddComponent<KeeperManager>();

            RewardSystemManager.Init();

            InitTeacherForPlayer();
            GameSettings.HighQualityGfx = false;
        }

        /// <summary>
        /// New profile entry point
        /// </summary>
        public void InitTeacherForPlayer()
        {
            if (Player == null)
                Player = new PlayerProfile();

            DB = new DatabaseManager(GameSettings.UseTestDatabase, Player);
            Teacher = new TeacherAI(DB, Player);

            if (GameLauncher == null)
                GameLauncher = new MiniGameLauncher(Teacher);

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

        #region Reset

        public void ResetCurrentPlayer()
        {
            var playerId = PlayerProfileManager.CurrentPlayer;

            // Delete DB
            DB.DropProfile();
            I.DB = null;

            PlayerProfileManager.DeleteCurrentPlayer();

            // AppManager.I.PlayerProfileManager.DeleteAllProfiles();
            NavigationManager.I.GoHome();
            Debug.Log("Reset current player: " + playerId);
        }

        public void ResetEverything()
        {
            // Reset all the Databases
            foreach (var playerId in AppManager.I.Modules.PlayerProfile.Options.AvailablePlayers) {
                Debug.Log(playerId);
                DB.LoadDynamicDbForPlayerProfile(int.Parse(playerId));
                DB.DropProfile();
            }
            DB = null;

            // Reset all profiles (from SRDebugOptions)
            PlayerPrefs.DeleteAll();
            AppManager.I.GameSettings.AvailablePlayers = new System.Collections.Generic.List<string>();
            AppManager.I.PlayerProfileManager.SaveGameSettings();
            SRDebug.Instance.HideDebugPanel();
            AppManager.I.Modules.SceneModule.LoadSceneWithTransition(NavigationManager.I.GetSceneName(AppScene.Home));

            Debug.Log("Reset ALL players.");
        }
        #endregion

        #region Pause

        void OnApplicationPause(bool pauseStatus)
        {
            appIsPaused = pauseStatus;

            // app is pausing
            if (appIsPaused) {
                LogManager.I.LogInfo(InfoEvent.AppSuspend);
            }

            //app is resuming
            if (!appIsPaused) {
                LogManager.I.LogInfo(InfoEvent.AppResume);
            }
            AudioManager.I.OnAppPause(appIsPaused);
        }

        void OnApplicationFocus(bool hasFocus)
        {
            appIsPaused = !hasFocus;
        }

        #endregion
    }
}