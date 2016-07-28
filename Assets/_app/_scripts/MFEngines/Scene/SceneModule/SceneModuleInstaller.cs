using UnityEngine;
using ModularFramework.Core;
using ModularFramework.Modules;

namespace EA4S {
    public class SceneModuleInstaller : ModuleInstaller<ISceneModule> {
        public SceneModuleSettings settings;
        public override ISceneModule InstallModule() {
            var concreteInstance = new SceneModule() { Settings = settings };
            if (concreteInstance != null)
                ModuleActivationState = ModuleActivationStates.Activated;
            return concreteInstance;
        }
    }
}