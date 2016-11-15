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

        public new static AppManager Instance {
            get { return GameManager.Instance as AppManager; }
        }

        public TeacherAI Teacher;
        public DatabaseManager DB;
        public PlayerProfile Player;
        public MiniGameLauncher GameLauncher;
        public GameObject CurrentGameManagerGO;
        public Log.AppLogManager LogManager;

        #region Init

        public string IExist()
        {
            return "AppManager Exists";
        }

        public void InitDataAI()
        {

            if (Player == null)
                Player = new PlayerProfile();
            if (DB == null)
                DB = new DatabaseManager(GameSettings.UseTestDatabase, Player);
            if (Teacher == null)
                Teacher = new TeacherAI(DB, Player);
            if (GameLauncher == null)
                GameLauncher = new MiniGameLauncher(Teacher);

            gameObject.AddComponent<DebugManager>();
        }



        protected override void GameSetup()
        {
            base.GameSetup();
            gameObject.AddComponent<MiniGameAPI>();
            AdditionalSetup();
            InitDataAI();
            GameSettings.HighQualityGfx = false;
            //ResetProgressionData();


        }

        public PlayerProfileManager PlayerProfileManager = new PlayerProfileManager();

        private void AdditionalSetup()
        {
            // GameplayModule
            if (GetComponentInChildren<ModuleInstaller<IGameplayModule>>()) {
                IGameplayModule moduleInstance = GetComponentInChildren<ModuleInstaller<IGameplayModule>>().InstallModule();
                Modules.GameplayModule.SetupModule(moduleInstance, moduleInstance.Settings);
            }

            // PlayerProfileModule Install override
            //PlayerProfile.SetupModule(new PlayerProfileModuleDefault());

            AppManager.Instance.LogManager = new Log.AppLogManager();

            /* Player profile auto select first avatar or last selected */
            AppManager.Instance.GameSettings = new AppSettings() { AvailablePlayers = new List<string>() { } };
            AppManager.Instance.GameSettings = AppManager.Instance.PlayerProfile.LoadGlobalOptions<AppSettings>(new AppSettings()) as AppSettings;
            // If "GameSettings.LastActivePlayerId == 0" force here open player selection
            Player = new PlayerProfile().CreateOrLoadPlayerProfile(GameSettings.LastActivePlayerId == 0 ? "1" : GameSettings.LastActivePlayerId.ToString());
            Debug.Log("Active player: " + Player.Id);

        }

        /*void CachingLetterData()
        {
            foreach (var letterData in DB.GetAllLetterData()) {
                Letters.Add(new LL_LetterData(letterData.GetId()));
            }
        }*/

        #endregion

        #region Game Progression


        [HideInInspector]
        public bool IsAssessmentTime {
            get {
                return Player.CurrentJourneyPosition.PlaySession == 5;
            }
        }

        /// <summary>
        /// The current minigame.
        /// </summary>
        [HideInInspector]
        public Db.MiniGameData CurrentMinigame;


        public void ResetProgressionData()
        {
            Player.Reset();
        }

        /// <summary>
        /// Set result and return next scene name.
        /// </summary>
        /// <returns>return next scene name.</returns>
        public string MiniGameDone(string actualSceneName = "")
        {
            var returnString = "_Start";
            if (actualSceneName == "") {
                // from MiniGame

                if (Player.CurrentMiniGameInPlaySession >= (Teacher.CurrentPlaySessionMiniGames.Count - 1)) {
                    // end playsession
                    //Player.CurrentJourneyPosition.PlaySession = 0;
                    //Player.CurrentMiniGameInPlaySession = 0;
                    returnString = "app_Rewards";
                } else {
                    // next game in this playsession
                    Player.CurrentMiniGameInPlaySession++;
                    //Debug.Log("MiniGameDone PlaySessionGameDone = " + PlaySessionGameDone);
                    var myGameCode = TeacherAI.I.CurrentMiniGame.Code;
                    AppManager.Instance.GameLauncher.LaunchGame(myGameCode);
                }
            } else {
                // special cases
                if (actualSceneName == "assessment") {
                    Player.CurrentJourneyPosition.LearningBlock++;
                    Player.CurrentJourneyPosition.PlaySession = 1;
                    Player.CurrentMiniGameInPlaySession = 0;
                    returnString = "app_Map";
                }
                if (actualSceneName == "rewards") {
                    Player.CurrentJourneyPosition.PlaySession++;
                    Player.CurrentMiniGameInPlaySession = 0;
                    Debug.Log("New PlaySession = " + Player.CurrentJourneyPosition.PlaySession);
                    GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Map");
                }

            }
            return returnString;
        }

        ///// <summary>
        ///// Called when playSession value changes.
        ///// </summary>
        //        void OnPlaySessionValueChange()
        //        {
        //            LoggerEA4S.Log("app", "PlaySession", "changed", PlaySession.ToString());
        //            LoggerEA4S.Save();
        //        }

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