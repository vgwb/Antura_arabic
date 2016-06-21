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
using System.Collections.Generic;
using ModularFramework.Core;
using System;
using Google2u;
using Newtonsoft.Json;

namespace ModularFramework.Modules {
    /// <summary>
    /// Concrete implementation for module type LocalizationModule.
    /// </summary>
    public class LocalizationModuleGoogle2U : ILocalizationModule {
        public string ActualLanguage { get; set; }
        public string[] AvailableLanguages { get; set; }

        #region IModule Implementation
        public ILocalizationModule ConcreteModuleImplementation { get; set; }
        public IModuleSettings Settings { get; set; }
        
        public ILocalizationModule SetupModule(ILocalizationModule _concreteModule, IModuleSettings _settings = null) {
            Settings = _settings;
            AvailableLanguages = GetAllAvailableLanguages();
            ActualLanguage = AvailableLanguages[0];
            return this;
        }
        #endregion

        public string GetLocalizedString(string _locStringID) {
            LocalizationDataRow row = LocalizationData.Instance.GetRow(_locStringID);
            if (row == null)
                return string.Format("{0}{1}{2}", "[", _locStringID, "]");
            string returnString = row.GetStringData(ActualLanguage);
            return returnString;
        }

        public string SetActualLanguage(string _languageToSet) {
            ActualLanguage = _languageToSet;
            return ActualLanguage;
        }

        public string[] GetAllAvailableLanguages() {
            string[] returnArray = new string[] { };
            foreach (LanguagesRow lang in Languages.Instance.Rows) {
                if (lang._Enabled) { 
                    Array.Resize(ref returnArray, returnArray.Length+1);
                    returnArray[returnArray.Length - 1] = lang._Localization_ID;
                }
            }
            return returnArray;
        }

    }
    /// <summary>
    /// Settings for this implementation of DataModule.
    /// </summary>
    [Serializable]
    public class LocalizationModuleGoogle2USettings : IModuleSettings { }
}
