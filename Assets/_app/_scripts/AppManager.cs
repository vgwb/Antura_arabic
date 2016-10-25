using UnityEngine;
using System.Collections.Generic;
using ModularFramework.Core;
using ModularFramework.Modules;
using EA4S.API;

namespace EA4S
{
    public class AppManager : GameManager
    {
        new public AppSettings GameSettings = new AppSettings();

        new public static AppManager Instance {
            get { return GameManager.Instance as AppManager; }
        }

        /// <summary>
        /// Tmp var to store actual gameplay word already used.
        /// </summary>
        public List<LL_WordData> ActualGameplayWordAlreadyUsed = new List<LL_WordData>();
        public string ActualGame = string.Empty;

        public List<LL_LetterData> Letters = new List<LL_LetterData>();

        public TeacherAI Teacher;
        public DatabaseManager DB;
        public PlayerProfile Player;
        public GameObject CurrentGameManagerGO;

        #region Init

        public string IExist()
        {
            return "AppManager Exists";
        }

        public void InitDataAI()
        {

            if (DB == null)
                DB = new DatabaseManager(); 
            if (Player == null)
                Player = new PlayerProfile();
            if (Teacher == null)
                Teacher = new TeacherAI(this.DB, this.Player);
        }

        protected override void GameSetup()
        {
            base.GameSetup();

            gameObject.AddComponent<MiniGameAPI>();

            AdditionalSetup();

            InitDataAI();

            CachingLetterData();

            GameSettings.HighQualityGfx = false;

            ResetProgressionData();

//            this.ObserveEveryValueChanged(x => PlaySession).Subscribe(_ => {
//                OnPlaySessionValueChange();
//            });

        }

        void AdditionalSetup()
        {
            // GameplayModule
            if (GetComponentInChildren<ModuleInstaller<IGameplayModule>>()) {
                IGameplayModule moduleInstance = GetComponentInChildren<ModuleInstaller<IGameplayModule>>().InstallModule();
                Modules.GameplayModule.SetupModule(moduleInstance, moduleInstance.Settings);
            }

            // PlayerProfileModule Install override
            PlayerProfile.SetupModule(new PlayerProfileModuleDefault());
        }

        void CachingLetterData()
        {
            foreach (var letterData in DB.GetAllLetterData()) {
                Letters.Add(new LL_LetterData(letterData.GetId()));
            }
        }

        #endregion

        #region Game Progression

        [HideInInspector]
        public int PlaySessionGameDone = 0;

        [HideInInspector]
        public bool IsAssessmentTime { get { return Player.CurrentJourneyPosition.PlaySession == 3; } }
        // Change this to change position of assessment in the alpha.
        [HideInInspector]
        public Db.MiniGameData ActualMinigame;


        public void ResetProgressionData()
        {
            PlaySessionGameDone = 0;
            Player.Reset();
        }

        /// <summary>
        /// Set result and return next scene name.
        /// </summary>
        /// <returns>return next scene name.</returns>
        public string MiniGameDone(string actualSceneName = "")
        {
            string returnString = "app_Start";
            if (actualSceneName == "") {
                if (PlaySessionGameDone > 0) { // end playsession
                    Player.CurrentJourneyPosition.PlaySession++;
                    PlaySessionGameDone = 0;
                    returnString = "app_Rewards";
                } else {
                    // next game in this playsession
                    PlaySessionGameDone++;
                    //Debug.Log("MiniGameDone PlaySessionGameDone = " + PlaySessionGameDone);
                    returnString = "app_Wheel";
                }
            } else {
                // special cases
                if (actualSceneName == "assessment") {
                    Player.CurrentJourneyPosition.PlaySession++;
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
            // reset for already used word.
            ActualGameplayWordAlreadyUsed = new List<LL_WordData>();
        }

        #endregion

    }

}