using UnityEngine;
using ModularFramework.Core;
using ModularFramework.Modules;
using EA4S.API;

namespace EA4S
{
    public class AppManager : GameManager
    {
        public new AppSettings GameSettings = new AppSettings();

        public static AppManager I {
            get { return GameManager.Instance as AppManager; }
        }

        public TeacherAI Teacher;
        public DatabaseManager DB;
        public PlayerProfile Player;
        public MiniGameLauncher GameLauncher;
        public GameObject CurrentGameManagerGO;
        public LogManager LogManager;
        public PlayerProfileManager PlayerProfileManager;

        [HideInInspector]
        public Db.MiniGameData CurrentMinigame;

        bool appIsPaused = false;

        #region Init

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

        #region settings behaviours

        public void ToggleQualitygfx()
        {
            GameSettings.HighQualityGfx = !GameSettings.HighQualityGfx;
            CameraGameplayController.I.EnableFX(GameSettings.HighQualityGfx);
        }

        #endregion

        #region event delegate

        public void OnMinigameStart()
        {
        }

        #endregion

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
        {// Reset all the Databases
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
    }

}