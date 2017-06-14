
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

namespace EA4S.Core
{
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
                    returnString += line + System.Environment.NewLine;
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
