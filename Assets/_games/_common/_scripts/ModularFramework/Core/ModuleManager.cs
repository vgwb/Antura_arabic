
using UnityEngine;
using System;
using System.Collections.Generic;
namespace EA4S.Core
{
    [Serializable]
    public class ModuleManager {

        public string LocalizationDataPath;

        [SerializeField]
        public UIModule UIModule = new UIModule();
        public LocalizationModule LocalizationModule = new LocalizationModule();
        public PlayerProfileModule PlayerProfile = new PlayerProfileModule() { MultipleProfileSupported = true };
        public DataModule DataModule = new DataModule();
        public GameplayModule GameplayModule = new GameplayModule();

        public void ModulesSetup() {
            UIModule.SetupModule(new UIModuleDefault());
            //LocalizationModule.SetupModule(new LocalizationModuleGoogle2U());
            //SceneModule.SetupModule(new SceneModuleDefault());
            //PlayerProfile.SetupModule(new PlayerProfileModuleDefault());
            // For Test 
            //DummyModule.SetupModule(new DummyModuleDefault());
        }

        /// <summary>
        /// Find module installer components for auto install with settings from unity editor.
        /// </summary>
        /// <param name="_gamemanagerGO"></param>
        public void ModuleAutoInstallerOverride(GameObject _gamemanagerGO) {
            // DataModule
            if (_gamemanagerGO.GetComponentInChildren<ModuleInstaller<IDataModule>>()) {
                IDataModule dm = _gamemanagerGO.GetComponentInChildren<ModuleInstaller<IDataModule>>().InstallModule();
                DataModule.SetupModule(dm, dm.Settings);
                //typeof(IMyInterface).IsAssignableFrom(typeof(MyType))
                //typeof().GetInterfaces().Contains(typeof(IMyInterface))
            }
            // LocalizationModule
            if (_gamemanagerGO.GetComponentInChildren<ModuleInstaller<ILocalizationModule>>()) {
                ILocalizationModule dm = _gamemanagerGO.GetComponentInChildren<ModuleInstaller<ILocalizationModule>>().InstallModule();
                LocalizationModule.SetupModule(dm, dm.Settings);
            }
            // PlayerProfileModule Install
            if (_gamemanagerGO.GetComponentInChildren<ModuleInstaller<IPlayerProfileModule>>()) {
                IPlayerProfileModule moduleInstance = _gamemanagerGO.GetComponentInChildren<ModuleInstaller<IPlayerProfileModule>>().InstallModule();
                PlayerProfile.SetupModule(moduleInstance, moduleInstance.Settings);
            }
            // UIModule Install
            if (_gamemanagerGO.GetComponentInChildren<ModuleInstaller<IUIModule>>()) {
                IUIModule moduleInstance = _gamemanagerGO.GetComponentInChildren<ModuleInstaller<IUIModule>>().InstallModule();
                UIModule.SetupModule(moduleInstance, moduleInstance.Settings);
            }
        }
    }
    
    /// <summary>
    /// Base abstract class for module installer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ModuleInstaller<T> : MonoBehaviour {
        public abstract T InstallModule();
        public ModuleActivationStates ModuleActivationState = ModuleActivationStates.NotActivated;
    }
    public enum ModuleActivationStates { NotActivated, Activated }
}
