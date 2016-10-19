using UnityEngine;
using System.Collections.Generic;
using ModularFramework.Core;
using ModularFramework.Modules;
using UniRx;
using EA4S.API;

namespace EA4S
{
    public class AppManager : GameManager
    {
        public const string AppVersion = "0.6.3a";

        new public AppSettings GameSettings = new AppSettings();

        new public static AppManager Instance {
            get { return GameManager.Instance as AppManager; }
        }

        /// <summary>
        /// Tmp var to store actual gameplay word already used.
        /// </summary>
        public List<WordData> ActualGameplayWordAlreadyUsed = new List<WordData>();
        public string ActualGame = string.Empty;

        public List<LetterData> Letters = new List<LetterData>();

        public TeacherAI Teacher;
        public DatabaseManager DB;
        public EA4S.PlayerProfile Player;
        public GameObject CurrentGameManagerGO;

        #region Init

        public string IExist()
        {
            return "AppManager Exists";
        }

        public void InitDataAI()
        {
            if (DB == null)
                DB = new DatabaseManager("1");  // @todo: player ID should be passed here
            if (Teacher == null)
                Teacher = new TeacherAI();
            if (Player == null)
                Player = new EA4S.PlayerProfile();
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

            this.ObserveEveryValueChanged(x => PlaySession).Subscribe(_ => {
                OnPlaySessionValueChange();
            });

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
            foreach (var letterData in DB.FindAllLetterData()) {
                Letters.Add(new LetterData(letterData.GetId()));
            }
        }

        #endregion

        #region Game Progression

        [HideInInspector]
        public int Stage = 2;
        [HideInInspector]
        public int LearningBlock = 4;
        [HideInInspector]
        public int PlaySession = 1;
        [HideInInspector]
        public int PlaySessionGameDone = 0;

        [HideInInspector]
        public bool IsAssessmentTime { get { return PlaySession == 3; } }
        // Change this to change position of assessment in the alpha.
        [HideInInspector]
        public Db.MiniGameData ActualMinigame;


        public void ResetProgressionData()
        {
            Stage = 2;
            LearningBlock = 4;
            PlaySession = 1;
            PlaySessionGameDone = 0;
            Player.Reset();
        }

        /// <summary>
        /// Give right game. Alpha version.
        /// </summary>
        public Db.MiniGameData GetMiniGameForActualPlaySession()
        {
            Db.MiniGameData miniGame = null;
            switch (PlaySession) {
                case 1:
                    if (PlaySessionGameDone == 0)
                        miniGame = DB.GetMiniGameDataById("FastCrowd_letter");
                    else
                        miniGame = DB.GetMiniGameDataById("Balloons_spelling");
                    break;
                case 2:
                    if (PlaySessionGameDone == 0)
                        miniGame = DB.GetMiniGameDataById("FastCrowd_words");
                    else
                        miniGame = DB.GetMiniGameDataById("Tobogan");
                    break;
                case 3:
                    miniGame = DB.GetMiniGameDataById("Assessment");
                    break;
            }
            ActualMinigame = miniGame;
            return miniGame;
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
                    PlaySession++;
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
                    PlaySession++;
                }
            }
            return returnString;
        }

        ///// <summary>
        ///// Called when playSession value changes.
        ///// </summary>
        void OnPlaySessionValueChange()
        {
            LoggerEA4S.Log("app", "PlaySession", "changed", PlaySession.ToString());
            LoggerEA4S.Save();
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
            // reset for already used word.
            ActualGameplayWordAlreadyUsed = new List<WordData>();
        }

        #endregion

    }

    /// <summary>
    /// Game Setting Extension class.
    /// </summary>
    [System.Serializable]
    public class AppSettings : GameSettings
    {
        public bool DoLogPlayerBehaviour;
        public bool HighQualityGfx;
    }

}