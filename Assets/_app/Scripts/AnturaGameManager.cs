using UnityEngine;
using System.Collections;
using ModularFramework.Core;
using ModularFramework.Modules;

public class AnturaGameManager : GameManager {

    

    protected override void GameSetup() {
        base.GameSetup();
        AdditionalSetup();
    }

    void AdditionalSetup() {
        // GameplayModule
        if (GetComponentInChildren<ModuleInstaller<IGameplayModule>>()) {
            IGameplayModule moduleInstance = GetComponentInChildren<ModuleInstaller<IGameplayModule>>().InstallModule();
            Modules.GameplayModule.SetupModule(moduleInstance, moduleInstance.Settings);
        }
    }
}
