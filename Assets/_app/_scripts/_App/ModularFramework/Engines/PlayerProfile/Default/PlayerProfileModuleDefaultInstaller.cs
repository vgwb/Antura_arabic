
using System;
using EA4S.Core;
using UnityEngine;

namespace EA4S.Core { 
    /*
    [AddComponentMenu("Modular Framework/Modules Installers/PlayerProfileModuleDefault")]
    public class PlayerProfileModuleDefaultInstaller : ModuleInstaller<IPlayerProfileModule> {
        public PlayerProfileModuleDefaultSettings settings;
        public override IPlayerProfileModule InstallModule() {
            var concreteInstance = new PlayerProfileModuleDefault() { Settings = settings };
            if (concreteInstance != null)
                ModuleActivationState = ModuleActivationStates.Activated;
            return concreteInstance;
        }
    }*/
}

/// <summary>
/// Settings for this implementation of PlayerProfileModule.
/// </summary>
[Serializable]
public class PlayerProfileModuleDefaultSettings : IModuleSettings {
    // Add Here setup settings properties (variables if you want to enable in editor)
}