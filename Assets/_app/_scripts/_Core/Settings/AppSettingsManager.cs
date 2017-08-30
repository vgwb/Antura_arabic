using UnityEngine;

namespace Antura.Core
{
    public class AppSettingsManager
    {
        private const string SETTINGS_PREFS_KEY = "OPTIONS";
        private AppSettings _settings = new AppSettings();

        public AppSettings Settings
        {
            get { return _settings; }
            set {
                if (value != _settings) {
                    _settings = value;
                    // Auto save at any change
                    SaveSettings();
                } else {
                    _settings = value;
                }
            }
        }

        /// <summary>
        /// Loads the settings. Creates new settings if none are found.
        /// </summary>
        public AppSettings LoadSettings()
        {
            if (PlayerPrefs.HasKey(SETTINGS_PREFS_KEY)) {
                var serializedObjs = PlayerPrefs.GetString(SETTINGS_PREFS_KEY);
                Settings = JsonUtility.FromJson<AppSettings>(serializedObjs);
            } else {
                Settings = new AppSettings();
            }
            return _settings;
        }

        /// <summary>
        /// Save all settings. This also saves player profiles.
        /// </summary>
        public void SaveSettings()
        {
            var serializedObjs = JsonUtility.ToJson(Settings);
            PlayerPrefs.SetString(SETTINGS_PREFS_KEY, serializedObjs);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Delete all settings. This also deletes all player profiles.
        /// </summary>
        public void DeleteAllSettings()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}