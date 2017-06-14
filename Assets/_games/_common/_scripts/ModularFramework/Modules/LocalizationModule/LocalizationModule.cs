
using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Modules System with strategy pattern implementation.
/// </summary>
namespace EA4S.Core
{
    /// <summary>
    /// This is the Context implementation of this Module type with all functionalities provided from the Strategy interface.
    /// </summary>
    public class LocalizationModule : ILocalizationModule {
        public string ActualLanguage {
            get {
                return ConcreteModuleImplementation.ActualLanguage;
            }
            set {
                if (ConcreteModuleImplementation.ActualLanguage != value) {
                    ConcreteModuleImplementation.ActualLanguage = value;
                    if (OnLangueChanged != null) {
                        OnLangueChanged();
                    }
                } else {
                    ConcreteModuleImplementation.ActualLanguage = value;
                }
            }
        }
        public string[] AvailableLanguages {
            get {
                return ConcreteModuleImplementation.AvailableLanguages;
            }
            set {
                ConcreteModuleImplementation.AvailableLanguages = value;
            }
        }

        #region IModule implementation
        /// <summary>
        /// Concrete Module Implementation.
        /// </summary>
        public ILocalizationModule ConcreteModuleImplementation { get; set; }
        public IModuleSettings Settings { get; set; }
        
        /// <summary>
        /// Module Setup.
        /// </summary>
        /// <param name="_concreteModule">Concrete module implementation to set as active module behaviour.</param>
        /// <returns></returns>
        public ILocalizationModule SetupModule(ILocalizationModule _concreteModule, IModuleSettings _settings = null) {
            ConcreteModuleImplementation = _concreteModule.SetupModule(_concreteModule, _settings);
            if (ConcreteModuleImplementation == null)
                OnSetupError();
            return ConcreteModuleImplementation;
        }

        /// <summary>
        /// Called if an error occurred during the setup.
        /// </summary>
        void OnSetupError() {
            Debug.LogErrorFormat("Module {0} setup return an error.", this.GetType());
        }
        #endregion

        #region API

        /// <summary>
        /// Get localized string in actual selected language.
        /// </summary>
        /// <param name="_locStringID">ID of localized string.</param>
        /// <returns></returns>
        public string GetLocalizedString(string _locStringID) {
            return ConcreteModuleImplementation.GetLocalizedString(_locStringID);
        }

        /// <summary>
        /// Language to set.
        /// </summary>
        /// <param name="_languageToSet"></param>
        /// <returns></returns>
        public string SetActualLanguage(string _languageToSet) {
            return ConcreteModuleImplementation.SetActualLanguage(_languageToSet);
        }

        /// <summary>
        /// Get all available languages.
        /// </summary>
        /// <returns></returns>
        public string[] GetAllAvailableLanguages() {
            return ConcreteModuleImplementation.GetAllAvailableLanguages();
        }

        #endregion

        #region Events
        public delegate void LocalizationEventHandler();
        /// <summary>
        /// Happens when actual language change.
        /// </summary>
        public static event LocalizationEventHandler OnLangueChanged;
        #endregion

    }

    /// <summary>
    /// Strategy interface. 
    /// Provide All the functionalities required for any Concrete implementation of the module.
    /// </summary>
    public interface ILocalizationModule : IModule<ILocalizationModule> {
        string ActualLanguage { get; set; }
        string[] AvailableLanguages { get; set; }

        string SetActualLanguage(string _languageToSet);
        string GetLocalizedString(string _locStringID);
        string[] GetAllAvailableLanguages();
    }
}