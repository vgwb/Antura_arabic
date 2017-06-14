using System.Collections.Generic;
using EA4S.Core;
using EA4S.Database;
using EA4S.Rewards;
using UnityEngine;
using System.Linq;

namespace EA4S.Profile
{
    /// <summary>
    /// Handles the creation, selection, and deletion of player profiles.
    /// </summary>
    public class PlayerProfileManager
    {
        /*
        const string SETTINGS_PREFS_KEY = "OPTIONS";

        private AppSettings _settings;

        private AppSettings Settings
        {
            get { return _settings; }
            set
            {
                if (value != _settings)
                {
                    _settings = value;
                    // Auto save at any change
                    SaveSettings();
                }
                else
                {
                    _settings = value;
                }
            }
        }

        /// <summary>
        /// Loads the app settings with default fallback value.
        /// </summary>
        private AppSettings LoadSettings(AppSettings _defaultSettings) 
        {
            if (PlayerPrefs.HasKey(SETTINGS_PREFS_KEY))
            {
                var serializedObjs = PlayerPrefs.GetString(SETTINGS_PREFS_KEY);
                Settings = JsonUtility.FromJson<AppSettings>(serializedObjs);
                return _settings;
            }
            else
            {
                // Players list not created yet.
                Settings = _defaultSettings;
                LoadSettings(_defaultSettings);
                SaveSettings();
                return _defaultSettings;
            }
        }

        /// <summary>
        /// Save all player profiles.
        /// </summary>
        private void SaveSettings()
        {
            string serializedObjs = JsonUtility.ToJson(Settings);
            PlayerPrefs.SetString(SETTINGS_PREFS_KEY, serializedObjs);
            PlayerPrefs.Save();
        }
        */


        #region Current Player

        private PlayerProfile _currentPlayer;
        /// <summary>
        /// Actual Player.
        /// </summary>
        public PlayerProfile CurrentPlayer {
            get { return _currentPlayer; }
            set {
                if (_currentPlayer != value)
                {
                    AppManager.I.Player = _currentPlayer = value;
                    AppManager.I.Teacher.SetPlayerProfile(value);
                    // refactor: make this part more clear, better create a SetCurrentPlayer() method for this!
                    if (AppManager.I.DB.HasLoadedPlayerProfile()) {
                        LogManager.I.LogInfo(InfoEvent.AppSessionEnd, "{\"AppSession\":\"" + LogManager.I.AppSession + "\"}");
                    }
                    AppManager.I.AppSettings.LastActivePlayerUUID = value.Uuid;
                    SaveGameSettings();
                    LogManager.I.LogInfo(InfoEvent.AppSessionStart, "{\"AppSession\":\"" + LogManager.I.AppSession + "\"}");
                    AppManager.I.NavigationManager.InitialisePlayerNavigationData(_currentPlayer);

                    _currentPlayer.LoadRewardsUnlockedFromDB(); // refresh list of unlocked rewards
                    if (OnProfileChanged != null)
                        OnProfileChanged();
                }
                _currentPlayer = value;
            }
        }

        #endregion

        #region API        
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerProfileManager"/> class.
        /// </summary>
        public PlayerProfileManager()
        {
            // ReloadGameSettings();
        }

        /// <summary>
        /// Reloads the game settings (AppSettings) from PlayerPrefs.
        /// TODO: rebuild database only for desynchronized profile
        /// </summary>
        public void ReloadGameSettings(bool alsoLoadCurrentPlayer = true)
        {
            AppManager.I.AppSettingsManager.LoadSettings(new AppSettings());
            //AppManager.I.AppSettings = new AppSettings() { };
            //AppManager.I.AppSettings = LoadSettings(new AppSettings()) as AppSettings;

            if (alsoLoadCurrentPlayer)
            {
                // No last active? Get the first one.
                if (AppManager.I.AppSettings.LastActivePlayerUUID == string.Empty) {
                    if (AppManager.I.AppSettings.SavedPlayers.Count > 0) {
                        //UnityEngine.Debug.Log("No last! Get the first.");
                        AppManager.I.AppSettings.LastActivePlayerUUID = AppManager.I.AppSettings.SavedPlayers[0].Uuid;
                    } else {
                        AppManager.I.Player = null;
                        Debug.Log("Actual Player == null!!");
                    }
                } else {
                    string playerUUID = AppManager.I.AppSettings.LastActivePlayerUUID;

                    // Check whether the SQL DB is in-sync first
                    PlayerProfileData profileFromDB = AppManager.I.DB.LoadDatabaseForPlayer(playerUUID);

                    // If null, the player does not actually exist.
                    // The DB got desyinced. Do not load it!
                    if (profileFromDB != null) {
                        //UnityEngine.Debug.Log("DB in sync! OK!");
                        SetPlayerAsCurrentByUUID(playerUUID);
                    } else {
                        //UnityEngine.Debug.Log("DB OUT OF SYNC. RESET");
                        ResetEverything();
                        ReloadGameSettings();
                    }
                }
            }
        }

        /// <summary>
        /// Creates the player profile.
        /// </summary>
        /// <param name="age">The age.</param>
        /// <param name="gender">The gender.</param>
        /// <param name="avatarID">The avatar identifier.</param>
        /// <param name="tint">The color.</param>
        /// <returns></returns>
        public string CreatePlayerProfile(int age, PlayerGender gender, int avatarID, PlayerTint tint, bool isDemoUser = false)
        {
            PlayerProfile returnProfile = new PlayerProfile();
            // Data
            returnProfile.Uuid = System.Guid.NewGuid().ToString();
            returnProfile.Age = age;
            returnProfile.Gender = gender;
            returnProfile.AvatarId = avatarID;
            returnProfile.Tint = tint;
            returnProfile.IsDemoUser = isDemoUser;
            returnProfile.ProfileCompletion = (isDemoUser ? ProfileCompletionState.GameCompletedAndFinalShowed : ProfileCompletionState.New);

            // DB Creation
            AppManager.I.DB.CreateDatabaseForPlayer(returnProfile.ToData());
            // Added to list
            AppManager.I.AppSettings.SavedPlayers.Add(returnProfile.GetPlayerIconData());
            // Set player profile as current player
            AppManager.I.PlayerProfileManager.CurrentPlayer = returnProfile as PlayerProfile;
            // Create new antura skin
            RewardSystemManager.UnlockFirstSetOfRewards();

            // Call Event Profile creation
            if (OnNewProfileCreated != null)
                OnNewProfileCreated();

            return returnProfile.Uuid;
        }

        public bool ExistsDemoUser()
        {
            bool demoUserExists = false;
            var playerList = GetSavedPlayers();
            foreach (var player in playerList) {
                if (player.IsDemoUser) {
                    demoUserExists = true;
                }
            }
            return demoUserExists;
        }

        /// <summary>
        /// Sets the player as current player profile loading from db by UUID.
        /// </summary>
        /// <param name="playerUUID">The player UUID.</param>
        /// <returns></returns>
        public PlayerProfile SetPlayerAsCurrentByUUID(string playerUUID)
        {
            PlayerProfile returnProfile = GetPlayerProfileByUUID(playerUUID);
            AppManager.I.PlayerProfileManager.CurrentPlayer = returnProfile;
            return returnProfile;
        }

        /// <summary>
        /// Gets the player profile from db by UUID.
        /// </summary>
        /// <param name="playerUUID">The player UUID.</param>
        /// <returns></returns>
        public PlayerProfile GetPlayerProfileByUUID(string playerUUID)
        {
            PlayerProfileData profileFromDB = AppManager.I.DB.LoadDatabaseForPlayer(playerUUID);

            // If null, the player does not exist.
            // The DB got desyinced. Remove this player!
            if (profileFromDB == null) {
                Debug.LogError("ERROR: no profile data for player UUID " + playerUUID);
            }

            return new PlayerProfile().FromData(profileFromDB);
        }

        /// <summary>
        /// Deletes the player profile.
        /// </summary>
        /// <param name="playerUUID">The player UUID.</param>
        /// <returns></returns>
        public PlayerProfile DeletePlayerProfile(string playerUUID)
        {
            PlayerProfile returnProfile = new PlayerProfile();
            // it prevents errors if rewards unlock coroutine is still running
            AppManager.I.StopAllCoroutines();
            // TODO: check if is necessary to hard delete DB
            SavedPlayerData savedPlayerData = GetSavedPlayers().Find(p => p.Uuid == playerUUID);
            if (savedPlayerData.Uuid == string.Empty)
                return null;
            // if setted as active player in gamesettings remove from it
            if (savedPlayerData.Uuid == AppManager.I.AppSettings.LastActivePlayerUUID) {
                // if possible set the first available player...
                SavedPlayerData newActiveSavedPlayer = GetSavedPlayers().Find(p => p.Uuid != playerUUID);
                if (newActiveSavedPlayer.Uuid != null) {
                    AppManager.I.PlayerProfileManager.SetPlayerAsCurrentByUUID(newActiveSavedPlayer.Uuid);
                } else {
                    // ...else set to null
                    AppManager.I.PlayerProfileManager._currentPlayer = null;
                }
            }
            AppManager.I.AppSettings.SavedPlayers.Remove(savedPlayerData);

            SaveGameSettings();
            return returnProfile;
        }

        #region Saved Player Profiles

        /// <summary>
        /// Return the list of existing player profiles.
        /// </summary>
        /// <returns></returns>
        public List<SavedPlayerData> GetSavedPlayers()
        {
            return AppManager.I.AppSettings.SavedPlayers;
        }

        /// <summary>
        /// Saves the player settings.
        /// </summary>
        /// <param name="_playerProfile">The player profile.</param>
        public void SavePlayerSettings(PlayerProfile _playerProfile)
        {
            AppManager.I.DB.UpdatePlayerProfileData(_playerProfile.ToData());
        }

        /// <summary>
        /// Updates the PlayerIconData for current player in list of SavedPlayers in GameSettings.
        /// </summary>
        public void UpdateCurrentPlayerIconDataInSettings()
        {
            for (int i = 0; i < AppManager.I.AppSettings.SavedPlayers.Count; i++) {
                if (AppManager.I.AppSettings.SavedPlayers[i].Uuid == _currentPlayer.Uuid) {
                    AppManager.I.AppSettings.SavedPlayers[i] = CurrentPlayer.GetPlayerIconData();
                }
            }
            SaveGameSettings();
        }

        #endregion

        /// <summary>
        /// Saves the game settings.
        /// </summary>
        public void SaveGameSettings()
        {
            //Settings = AppManager.I.AppSettings;
            //SaveSettings();
            AppManager.I.AppSettingsManager.SaveSettings();
        }

       /* /// <summary>
        /// WARNING! Deletes all profiles.
        /// </summary>
        public void DeleteAllProfiles()
        {
            //SaveSettings();
            AppManager.I.AppSettingsManager.SaveSettings();
        }*/

        /// <summary>
        /// Resets everything.
        /// </summary>
        public void ResetEverything()
        {
            // Reset all the Databases
            if (AppManager.I.AppSettings.SavedPlayers != null)
            {
                foreach (SavedPlayerData pp in AppManager.I.AppSettings.SavedPlayers)
                {
                    AppManager.I.DB.LoadDatabaseForPlayer(pp.Uuid);
                    AppManager.I.DB.DropProfile();
                }
            }
            AppManager.I.DB.UnloadCurrentProfile();

            // Reset all settings too
            AppManager.I.AppSettingsManager.DeleteAllSettings();
            //PlayerPrefs.DeleteAll();
            ReloadGameSettings(alsoLoadCurrentPlayer: false);
            //SaveGameSettings();
        }

        #endregion

        #region Events
        public delegate void ProfileEventHandler();

        /// <summary>
        /// Occurs when [on profile changed].
        /// </summary>
        public static event ProfileEventHandler OnProfileChanged;
        public static event ProfileEventHandler OnNewProfileCreated;
        #endregion
    }
}