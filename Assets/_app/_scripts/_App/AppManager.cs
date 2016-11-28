using UnityEngine;
using System;
using System.Collections.Generic;
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

            LogManager.I.LogInfo(InfoEvent.AppStarted);
        }

        public void InitTeacherForPlayer()
        {
            if (Player == null)
                Player = new PlayerProfile();
            if (DB == null)
                DB = new DatabaseManager(GameSettings.UseTestDatabase, Player);
            // TODO @michele ToCheck ref: https://trello.com/c/r40yCfw1
            //if (Teacher == null)
            Teacher = new TeacherAI(DB, Player);
            if (GameLauncher == null)
                GameLauncher = new MiniGameLauncher(Teacher);
        }
        #endregion

        #region Game Progression

        [HideInInspector]
        [Obsolete("incorrect and shoudl be moved to Teacher or Navigation manager", true)]
        public bool IsAssessmentTime {
            get {
                return Player.CurrentJourneyPosition.PlaySession == 5;
            }
        }


        public void ResetProgressionData()
        {
            Player.Reset();
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

    }

}