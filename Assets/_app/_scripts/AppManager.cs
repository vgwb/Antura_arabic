using UnityEngine;
using System.Collections.Generic;
using ModularFramework.Core;
using ModularFramework.Modules;
using ModularFramework.Helpers;
using Google2u;
using UniRx;
using EA4S;

namespace EA4S
{
    public class AppManager : GameManager
    {
        public const string AppVersion = "0.5.7";

        new public AppSettings GameSettings = new AppSettings();

        new public static AppManager Instance
        {
            get { return GameManager.Instance as AppManager; }
        }

        /// <summary>
        /// Tmp var to store actual gameplay word already used.
        /// </summary>
        public List<WordData> ActualGameplayWordAlreadyUsed = new List<WordData>();

        public string ActualGame = string.Empty;

        #region Mood

        /// <summary>
        /// False if not executed start mood eval.
        /// </summary>
        [HideInInspector]
        public bool StartMood = false;
        /// <summary>
        /// Start Mood value. Values 0,1,2,3,4.
        /// </summary>
        [HideInInspector]
        public int StartMoodEval = 0;
        /// <summary>
        /// End Mood value. Values 0,1,2,3,4.
        /// </summary>
        [HideInInspector]
        public int EndMoodEval = 0;

        #endregion

        public List<LetterData> Letters = new List<LetterData>();
        
        public TeacherAI Teacher;
        public Database DB;

        public GameObject CurrentGameManagerGO;

        #region Init

        public string IExist()
        {
            return "AppManager Exists";
        }

        public void InitDataAI()
        {
            if (DB == null)
                DB = new Database();
            if (Teacher == null)
                Teacher = new TeacherAI();
        }

        protected override void GameSetup()
        { 
            base.GameSetup();

            AdditionalSetup();

            CachingLetterData();

            GameSettings.HighQualityGfx = false;

            InitDataAI();

            ResetProgressionData();

            this.ObserveEveryValueChanged(x => PlaySession).Subscribe(_ =>
                {
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


        }

        void CachingLetterData()
        {
            foreach (string rowName in letters.Instance.rowNames) {
                lettersRow letRow = letters.Instance.GetRow(rowName);
                Letters.Add(new LetterData(rowName, letRow));
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
        public MinigameData ActualMinigame;


        public void ResetProgressionData()
        {
            Stage = 2;
            LearningBlock = 4;
            PlaySession = 1;
            PlaySessionGameDone = 0;
        }

        /// <summary>
        /// Give right game. Alpha version.
        /// </summary>
        public MinigameData GetMiniGameForActualPlaySession()
        {
            MinigameData miniGame = null;
            switch (PlaySession) {
                case 1:
                    if (PlaySessionGameDone == 0)
                        miniGame = DB.gameData.Find(g => g.Code == "fastcrowd");
                    else
                        miniGame = DB.gameData.Find(g => g.Code == "balloons");
                    break;
                case 2:
                    if (PlaySessionGameDone == 0)
                        miniGame = DB.gameData.Find(g => g.Code == "fastcrowd_words");
                    else
                        miniGame = DB.gameData.Find(g => g.Code == "dontwakeup");
                    break;
                case 3:
                    miniGame = new MinigameData("Assessment", "Assessment", "Assessment", "app_Assessment", true);
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