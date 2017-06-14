using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.Core
{
    public class PlayerProfileModule 
    {
        #region IModule Implementation

        const string OPTIONS_PREFS_KEY = "OPTIONS";

        private GlobalOptions options;

        public GlobalOptions Options
        {
            get { return options; }
            set
            {
                if (value != options)
                {
                    options = value;
                    // Auto save at any change
                    SaveAllOptions();
                }
                else
                {
                    options = value;
                }
            }
        }

        #endregion

        /// <summary>
        /// Loads the global options with default fallback value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_defaultOptions">The default options.</param>
        /// <returns></returns>
        public GlobalOptions LoadGlobalOptions<T>(T _defaultOptions) where T : GlobalOptions
        {
            string serializedObjs;
            if (PlayerPrefs.HasKey(OPTIONS_PREFS_KEY))
            {
                serializedObjs = PlayerPrefs.GetString(OPTIONS_PREFS_KEY);
                Options = JsonUtility.FromJson<T>(serializedObjs);
                return options;
            }
            else
            {
                // Players list not created yet.
                Options = _defaultOptions;
                LoadGlobalOptions<T>(_defaultOptions);
                SaveAllOptions();
                return _defaultOptions;
            }
        }

        /// <summary>
        /// Save all player profiles.
        /// </summary>
        public void SaveAllOptions()
        {
            string serializedObjs = JsonUtility.ToJson(Options);
            PlayerPrefs.SetString(OPTIONS_PREFS_KEY, serializedObjs);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// WARNING! Delete all stored profiles and set actual profile to null.
        /// </summary>
        public void DeleteAllPlayerProfiles()
        {
            SaveAllOptions();
            //ActivePlayer = null;
        }

    }

    public interface IPlayerProfile
    {
        string Key { get; set; }
    }
}