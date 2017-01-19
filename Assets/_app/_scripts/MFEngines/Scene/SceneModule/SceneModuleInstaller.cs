using ModularFramework.Core;
using ModularFramework.Modules;

namespace EA4S
{
    /// <summary>
    /// Installer for the SceneModule.
    /// <seealso cref="SceneModule"/>
    /// </summary>
    // refactor: can we remove the ModularFramerwork?
    public class SceneModuleInstaller : ModuleInstaller<ISceneModule>
    {
        public SceneModuleSettings settings;
        public override ISceneModule InstallModule() {
            var concreteInstance = new SceneModule() { Settings = settings };
            if (concreteInstance != null)
                ModuleActivationState = ModuleActivationStates.Activated;
            return concreteInstance;
        }
    }
}