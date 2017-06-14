using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.Core
{
    public partial class PlayerProfileModule : IPlayerProfileModule
    {
        public IPlayerProfile ActivePlayer { get; set; }

        #region IModule Implementation

        public IPlayerProfileModule ConcreteModuleImplementation { get; set; }
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

        public IModuleSettings Settings { get; set; }

        public IPlayerProfileModule SetupModule(IPlayerProfileModule _concreteModule, IModuleSettings _settings = null)
        {
            Settings = _settings;
            // Add Here setup stuffs for this concrete implementation
            return this;
        }

        #endregion

        /// <summary>
        /// Create new player profile, if not exist already, and save updated list of available players on PlayerPrefs.
        /// </summary>
        /// <param name="_newPlayer"></param>
        /// <param name="_extProfile"></param>
        /// <returns></returns>
        public IPlayerProfile CreateNewPlayer(IPlayerProfile _newPlayer, IPlayerExtendedProfile _extProfile = null)
        {
            return null;
        }

        /// <summary>
        /// Delete player.
        /// </summary>
        /// <param name="_playerId"></param>
        public void DeletePlayer(string _playerId)
        {
        }

        /// <summary>
        /// Load player settings.
        /// </summary>
        /// <param name="_playerId"></param>
        public IPlayerProfile LoadPlayerSettings<T>(string _playerId) where T : IPlayerProfile
        {
            if (PlayerPrefs.HasKey(GetStoreKeyForPlayer(_playerId)))
            {
                string serializableProfile = PlayerPrefs.GetString(GetStoreKeyForPlayer(_playerId));
                IPlayerProfile returnProfile = JsonUtility.FromJson<T>(serializableProfile);
                returnProfile.Key = _playerId;
                return returnProfile;
            }
            else
            {
                Debug.LogFormat("Profile {0} not found.", _playerId);
            }
            return null;
        }

        /// <summary>
        /// Save player settings on PlayerPrefs (do not update list of players and global options).
        /// </summary>
        /// <param name="_newPlayer"></param>
        /// <param name="_extProfile"></param>
        public void SavePlayerSettings(IPlayerProfile _newPlayer, IPlayerExtendedProfile _extProfile = null)
        {
            string storeKey = GetStoreKeyForPlayer(_newPlayer.Key);
            string serializedObjs = JsonUtility.ToJson(_newPlayer);
            if (serializedObjs != null)
                PlayerPrefs.SetString(storeKey, serializedObjs);
            else
                Debug.Log("Unable to serialize player profile : " + _newPlayer.Key);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Set the active player.
        /// </summary>
        /// <param name="_playerId"></param>
        public void SetActivePlayer<T>(string _playerId) where T : IPlayerProfile
        {
            IPlayerProfile pp = LoadPlayerSettings<T>(_playerId);
            if (pp == null)
            {
                Debug.LogError("Player not found");
            }
            else
            {
                ActivePlayer = pp;
            }
        }

        /// <summary>
        /// Update player settings (and if, extended profile).
        /// </summary>
        /// <param name="_newPlayer"></param>
        /// <param name="_extProfile"></param>
        /// <returns></returns>
        public IPlayerProfile UpdatePlayer(IPlayerProfile _newPlayer, IPlayerExtendedProfile _extProfile = null)
        {
            SavePlayerSettings(_newPlayer);
            return _newPlayer;
        }

        ///// <summary>
        ///// Loads the global options.
        ///// </summary>
        ///// <returns></returns>
        //public IGlobalOptions LoadGlobalOptions<T>() where T : IGlobalOptions {
        //    return LoadGlobalOptions<T>(Activator.CreateInstance<T>());
        //}

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

        #region Data Store

        const string OPTIONS_PREFS_KEY = "OPTIONS";
        const string PLAYER_PREFS_KEY = "PLAYER";

        /// <summary>
        /// Return correct player pref key.
        /// </summary>
        /// <param name="_playerId"></param>
        /// <returns></returns>
        static string GetStoreKeyForPlayer(string _playerId)
        {
            return string.Format("{0}_{1}", PLAYER_PREFS_KEY, _playerId);
        }

        #endregion

        /// <summary>
        /// WARNING! Delete all stored profiles and set actual profile to null.
        /// </summary>
        public void DeleteAllPlayerProfiles()
        {
            SaveAllOptions();
            ActivePlayer = null;
        }

    }

    /// <summary>
    /// Interface for optional Extended player profile.
    /// </summary>
    public interface IPlayerExtendedProfile
    {
        string PlayerRef { get; set; }
    }

    public interface IPlayerProfile
    {
        string Key { get; set; }
    }
}