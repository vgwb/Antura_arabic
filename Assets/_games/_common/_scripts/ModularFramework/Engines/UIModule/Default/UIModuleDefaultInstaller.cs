
using System;
using UnityEngine;

namespace EA4S.Core
{
    [AddComponentMenu("Modular Framework/Modules Installers/UI Default")]
    public class UIModuleDefaultInstaller : ModuleInstaller<IUIModule> {
        public UIModuleDefaultSettings settings;
        public override IUIModule InstallModule() {
            var concreteInstance = new UIModuleDefault() { Settings = settings };
            if (concreteInstance != null)
                ModuleActivationState = ModuleActivationStates.Activated;
            return concreteInstance;
        }
    }

    /// <summary>
    /// Settings for this implementation of UIModule.
    /// </summary>
    [Serializable]
    public class UIModuleDefaultSettings : IModuleSettings {
        // Add Here setup settings properties (variables if you want to enable in editor)
        public UIManager UIManager;
    }
}
