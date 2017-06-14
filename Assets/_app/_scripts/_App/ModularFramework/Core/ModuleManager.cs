using UnityEngine;
using System;

namespace EA4S.Core
{
    [Serializable]
    public class ModuleManager {

        [SerializeField]
        public PlayerProfileModule PlayerProfile = new PlayerProfileModule() { MultipleProfileSupported = true };
        public DataModule DataModule = new DataModule();

        public void ModuleAutoInstallerOverride(GameObject _gamemanagerGO) {
            // DataModule
            if (_gamemanagerGO.GetComponentInChildren<ModuleInstaller<IDataModule>>()) {
                IDataModule dm = _gamemanagerGO.GetComponentInChildren<ModuleInstaller<IDataModule>>().InstallModule();
                DataModule.SetupModule(dm, dm.Settings);
            }
            // PlayerProfileModule Install
            if (_gamemanagerGO.GetComponentInChildren<ModuleInstaller<IPlayerProfileModule>>()) {
                IPlayerProfileModule moduleInstance = _gamemanagerGO.GetComponentInChildren<ModuleInstaller<IPlayerProfileModule>>().InstallModule();
                PlayerProfile.SetupModule(moduleInstance, moduleInstance.Settings);
            }
        }
    }
    
    public abstract class ModuleInstaller<T> : MonoBehaviour {
        public abstract T InstallModule();
        public ModuleActivationStates ModuleActivationState = ModuleActivationStates.NotActivated;
    }
    public enum ModuleActivationStates { NotActivated, Activated }
}
