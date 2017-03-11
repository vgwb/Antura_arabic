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

        #region Properties

        private PlayerProfile currentPlayer;
        /// <summary>
        /// Actual Player.
        /// </summary>
        public PlayerProfile CurrentPlayer {
            get { return currentPlayer; }
            set {
                if (currentPlayer != value) {
                    AppManager.I.Player = currentPlayer = value;
                    AppManager.I.Teacher.SetPlayerProfile(value);
                    // refactor: make this part more clear, better create a SetCurrentPlayer() method for this!
                    if (AppManager.I.DB.HasLoadedPlayerProfile()) {
                        LogManager.I.LogInfo(InfoEvent.AppSessionEnd, "{\"AppSession\":\"" + LogManager.I.AppSession + "\"}");
                    }
                    AppManager.I.GameSettings.LastActivePlayerUUID = value.Uuid;
                    SaveGameSettings();
                    LogManager.I.LogInfo(InfoEvent.AppSessionStart, "{\"AppSession\":\"" + LogManager.I.AppSession + "\"}");
                    AppManager.I.NavigationManager.InitialisePlayerNavigationData(currentPlayer);

                    currentPlayer.LoadRewardsUnlockedFromDB(); // refresh list of unlocked rewards
                    if (OnProfileChanged != null)
                        OnProfileChanged();
                }
                currentPlayer = value;


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
            AppManager.I.GameSettings = new AppSettings() { AvailablePlayers = new List<string>() { } };
            AppManager.I.GameSettings = AppManager.I.PlayerProfile.LoadGlobalOptions<AppSettings>(new AppSettings()) as AppSettings;

            if (alsoLoadCurrentPlayer) {
                // No last active? Get the first one.
                if (AppManager.I.GameSettings.LastActivePlayerUUID == string.Empty) {
                    if (AppManager.I.GameSettings.SavedPlayers.Count > 0) {
                        //UnityEngine.Debug.Log("No last! Get the first.");
                        AppManager.I.GameSettings.LastActivePlayerUUID = AppManager.I.GameSettings.SavedPlayers[0].Uuid;
                    } else {
                        AppManager.I.Player = null;
                        Debug.Log("Actual Player == null!!");
                    }
                } else {
                    string playerUUID = AppManager.I.GameSettings.LastActivePlayerUUID;

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
            AppManager.I.GameSettings.SavedPlayers.Add(returnProfile.GetPlayerIconData());
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
                UnityEngine.Debug.LogError("ERROR: no profile data for player UUID " + playerUUID);
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
            PlayerIconData playerIconData = GetSavedPlayers().Find(p => p.Uuid == playerUUID);
            if (playerIconData.Uuid == string.Empty)
                return null;
            // if setted as active player in gamesettings remove from it
            if (playerIconData.Uuid == AppManager.I.GameSettings.LastActivePlayerUUID) {
                // if possible set the first available player...
                PlayerIconData newActivePlayer = GetSavedPlayers().Find(p => p.Uuid != playerUUID);
                if (newActivePlayer.Uuid != null) {
                    AppManager.I.PlayerProfileManager.SetPlayerAsCurrentByUUID(newActivePlayer.Uuid);
                } else {
                    // ...else set to null
                    AppManager.I.PlayerProfileManager.currentPlayer = null;
                }
            }
            AppManager.I.GameSettings.SavedPlayers.Remove(playerIconData);

            SaveGameSettings();
            return returnProfile;
        }

        /// <summary>
        /// Return the list of existing player profiles.
        /// </summary>
        /// <returns></returns>
        public List<PlayerIconData> GetSavedPlayers()
        {
            return AppManager.I.GameSettings.SavedPlayers;
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
            for (int i = 0; i < AppManager.I.GameSettings.SavedPlayers.Count; i++) {
                if (AppManager.I.GameSettings.SavedPlayers[i].Uuid == currentPlayer.Uuid) {
                    AppManager.I.GameSettings.SavedPlayers[i] = CurrentPlayer.GetPlayerIconData();
                }
            }
            SaveGameSettings();
        }

        /// <summary>
        /// Saves the game settings.
        /// </summary>
        public void SaveGameSettings()
        {
            AppManager.I.Modules.PlayerProfile.Options = AppManager.I.GameSettings;
            AppManager.I.Modules.PlayerProfile.SaveAllOptions();
        }

        /// <summary>
        /// WARNING! Deletes all profiles.
        /// </summary>
        public void DeleteAllProfiles()
        {
            AppManager.I.Modules.PlayerProfile.DeleteAllPlayerProfiles();
        }

        /// <summary>
        /// Resets the everything.
        /// </summary>
        public void ResetEverything()
        {
            // Reset all the Databases
            if (AppManager.I.GameSettings.SavedPlayers != null) {
                foreach (PlayerIconData pp in AppManager.I.GameSettings.SavedPlayers) {
                    //UnityEngine.Debug.Log(pp);
                    AppManager.I.DB.LoadDatabaseForPlayer(pp.Uuid);
                    AppManager.I.DB.DropProfile();
                }
            }
            AppManager.I.DB.UnloadCurrentProfile();

            // Reset all settings too
            UnityEngine.PlayerPrefs.DeleteAll();
            ReloadGameSettings(alsoLoadCurrentPlayer: false);
            SaveGameSettings();
        }

        #endregion

        #region events
        public delegate void ProfileEventHandler();

        /// <summary>
        /// Occurs when [on profile changed].
        /// </summary>
        public static event ProfileEventHandler OnProfileChanged;
        public static event ProfileEventHandler OnNewProfileCreated;
        #endregion
    }
}