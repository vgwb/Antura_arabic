using UnityEngine;
using System.Collections.Generic;
using ModularFramework.Core;
using ModularFramework.Modules;
using Google2u;
using CGL.Antura;

public class AnturaGameManager : GameManager {

    new public static AnturaGameManager Instance {
        get { return GameManager.Instance as AnturaGameManager; }
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
