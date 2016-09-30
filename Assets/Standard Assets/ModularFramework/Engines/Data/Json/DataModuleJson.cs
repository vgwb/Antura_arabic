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
using System;
using System.IO;
using System.Collections.Generic;
using ModularFramework.Core;

namespace ModularFramework.Modules {
    /// <summary>
    /// Concrete implementation for module type DataModule.
    /// </summary>
    public class DataModuleJson : IDataModule {

        #region IModule Implementation
        public IDataModule ConcreteModuleImplementation { get; set; }
        public IModuleSettings Settings { get; set; }

        public IDataModule SetupModule(IDataModule _concreteModule, IModuleSettings _settings = null) {
            Settings = _settings;
            Debug.Log("AutoSetup. DataPath: " + ((DataModuleJsonSettings)Settings).StoreDataPath);
            // Add Here setup stuffs for this concrete implementation
            return this;
        }
        #endregion

        /// <summary>
        /// Save data to storage.
        /// </summary>
        /// <param name="_dataToStore"></param>
        public void SaveData(ISerializableData _dataToStore, string _filename) {
            string serializedObject = JsonUtility.ToJson(_dataToStore);
            SaveDataToTextFile(serializedObject, _filename);
        }

        /// <summary>
        /// Load data from store and 
        /// </summary>
        /// <returns></returns>
        public ISerializableData LoadData(string _resourceName) {
            ISerializableData returnObject;
            DataModuleJsonSettings typedSettings = ((DataModuleJsonSettings)Settings);
            string stringFromResources = LoadStringFromTextFileResource(typedSettings.StoreDataPath + Path.PathSeparator + _resourceName);
            returnObject = JsonUtility.FromJson<ISerializableData>(stringFromResources);
            return returnObject;
        }


        /// <summary>
        /// TODO:
        /// </summary>
        /// <param name="_filename"></param>
        /// <returns></returns>
        IEnumerator<string> LoadAsyncStringFromTextFileResource(string _filename) {
            string returnString = string.Empty;

            using (StreamReader sr = new StreamReader(_filename)) {
                string line;

                while ((line = sr.ReadLine()) != null) {
                    returnString += line + Environment.NewLine;
                    yield return returnString;
                }
            }
            yield break;
        }

        string LoadStringFromTextFileResource(string _filename) {
            string returnString = string.Empty;
            using (StreamReader sr = new StreamReader(_filename)) {
                returnString = sr.ReadToEnd();
            }
            return returnString;
        }

        void SaveDataToTextFile(string _stringDataToWrite, string _filename, bool _append = false) {
            using (StreamWriter sw = new StreamWriter(_filename)) {
                sw.Write(_stringDataToWrite);
            }
        }
    }
}
