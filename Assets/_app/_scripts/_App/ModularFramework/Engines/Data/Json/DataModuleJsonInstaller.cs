
using UnityEngine;
using System.Collections;
using System;

namespace EA4S.Core
{
    [AddComponentMenu("Modular Framework/Modules Installers/DataModuleJson")]
    public class DataModuleJsonInstaller : ModuleInstaller<IDataModule>  {
        public DataModuleJsonSettings settings;
        public override IDataModule InstallModule() {
            var concreteInstance = new DataModuleJson() { Settings = settings };
            if (concreteInstance != null)
                ModuleActivationState = ModuleActivationStates.Activated;
            return concreteInstance;
        }
    }

    /// <summary>
    /// Settings for this implementation of DataModule.
    /// </summary>
    [Serializable]
    public class DataModuleJsonSettings : IModuleSettings {
        /// <summary>
        /// Public to be setted by editor.
        /// </summary>
        public string storeDataPath = "Data";

        public string StoreDataPath {
            get { return string.Format("{0}{1}{2}", Application.persistentDataPath, System.IO.Path.DirectorySeparatorChar, storeDataPath); }
            //set { storeDataPath = value; }
        }
    }

}
