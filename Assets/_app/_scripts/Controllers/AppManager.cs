using UnityEngine;
using System.Collections.Generic;
using ModularFramework.Core;
using ModularFramework.Modules;
using Google2u;
using EA4S;

namespace EA4S
{
    public class AppManager : GameManager
    {

        #region properties, variables and constants

        #region Overrides
        new public AppSettings GameSettings = new AppSettings();

        new public static AppManager Instance
        {
            get { return GameManager.Instance as AppManager; }
        }
        #endregion

        #region TMP

        /// <summary>
        /// Tmp var to store actual gameplay word already used.
        /// </summary>
        public List<WordData> ActualGameplayWordAlreadyUsed = new List<WordData>();

        public string ActualGame = string.Empty;

        #endregion

        #region Mood
        /// <summary>
        /// False if not executed start mood eval.
        /// </summary>
        public bool StartMood = false;
        #endregion

        #endregion

        public List<LetterData> Letters = new List<LetterData>();
        
        public TeacherAI Teacher;
        public Database DB;

        public const string AppVersion = "0.2.0";

        public string IExist() {
            return "AppManager Exists";
        }

        public void InitDataAI() {
            if (DB == null)
                DB = new Database();
            if (Teacher == null)
                Teacher = new TeacherAI();
        }

        protected override void GameSetup() { 
            base.GameSetup();

            AdditionalSetup();

            CachingLetterData();

            GameSettings.HighQualityGfx = true;
        }

        void AdditionalSetup() {
            // GameplayModule
            if (GetComponentInChildren<ModuleInstaller<IGameplayModule>>()) {
                IGameplayModule moduleInstance = GetComponentInChildren<ModuleInstaller<IGameplayModule>>().InstallModule();
                Modules.GameplayModule.SetupModule(moduleInstance, moduleInstance.Settings);
            }

            // SceneModule Install
            if (GetComponentInChildren<ModuleInstaller<ISceneModule>>()) {
                ISceneModule moduleInstance = GetComponentInChildren<ModuleInstaller<ISceneModule>>().InstallModule();
                Modules.SceneModule.SetupModule(moduleInstance, moduleInstance.Settings);
            }
        }

        void CachingLetterData() {
            foreach (string rowName in letters.Instance.rowNames) {
                lettersRow letRow = letters.Instance.GetRow(rowName);
                Letters.Add(new LetterData(rowName, letRow));
            }
        }

        public void ToggleQualitygfx() {
            GameSettings.HighQualityGfx = !GameSettings.HighQualityGfx;
            CameraGameplayController.I.EnableFX(GameSettings.HighQualityGfx);
        }

        public void OnMinigameStart() {
            // reset for already used word.
            ActualGameplayWordAlreadyUsed = new List<WordData>();
        }

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