using System;

namespace EA4S.Core
{
    /// <summary>
    /// Installer for the SceneModule.
    /// <seealso cref="GameplayModuleAnturaGameplay"/>
    /// </summary>
    // refactor: can we remove the ModularFramerwork?
    public class AnturaGameplayInstaller : ModuleInstaller<IGameplayModule>
    {
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