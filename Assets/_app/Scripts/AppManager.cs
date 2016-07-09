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

        new public static AppManager Instance
        {
            get { return GameManager.Instance as AppManager; }
        }

        public List<LetterData> Letters = new List<LetterData>();


        protected override void GameSetup() {
            base.GameSetup();
            AdditionalSetup();

            CachingLetterData();
        }

        void AdditionalSetup() {
            // GameplayModule
            if (GetComponentInChildren<ModuleInstaller<IGameplayModule>>()) {
                IGameplayModule moduleInstance = GetComponentInChildren<ModuleInstaller<IGameplayModule>>().InstallModule();
                Modules.GameplayModule.SetupModule(moduleInstance, moduleInstance.Settings);
            }
        }

        void CachingLetterData() {
            foreach (string rowName in letters.Instance.rowNames) {
                lettersRow letRow = letters.Instance.GetRow(rowName);
                Letters.Add(new LetterData(rowName, letRow));
            }
        }
    }
}