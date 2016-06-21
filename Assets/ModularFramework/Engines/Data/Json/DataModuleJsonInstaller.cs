/* --------------------------------------------------------------
*   Indie Contruction : Modular Framework for Unity
*   Copyright(c) 2016 Indie Construction / Paolo Bragonzi
*   All rights reserved. 
*   For any information refer to http://www.indieconstruction.com
*   
*   This library is free software; you can redistribute it and/or
*   modify it under the terms of the GNU Lesser General Public
*   License as published by the Free Software Foundation; either
*   version 3.0 of the License, or(at your option) any later version.
*   
*   This library is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
*   Lesser General Public License for more details.
*   
*   You should have received a copy of the GNU Lesser General Public
*   License along with this library.
* -------------------------------------------------------------- */
using UnityEngine;
using System.Collections;
using ModularFramework.Core;
using System;

namespace ModularFramework.Modules {
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
