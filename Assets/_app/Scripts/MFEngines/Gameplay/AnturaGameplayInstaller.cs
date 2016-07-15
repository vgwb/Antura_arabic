using System;
using ModularFramework.Core;
using ModularFramework.Modules;

namespace EA4S {

    public class AnturaGameplayInstaller : ModuleInstaller<IGameplayModule> {
        public AnturaGameplayInstallerSettings settings;
        public override IGameplayModule InstallModule() {
            var concreteInstance = new GameplayModuleAnturaGameplay() { Settings = settings };
            if (concreteInstance != null)
                ModuleActivationState = ModuleActivationStates.Activated;
            return concreteInstance;
        }
    }


    /// <summary>
    /// Settings for this implementation of GameplayModule.
    /// </summary>
    [Serializable]
    public class AnturaGameplayInstallerSettings : IModuleSettings {
        // Add Here setup settings properties (variables if you want to enable in editor)
    }
}