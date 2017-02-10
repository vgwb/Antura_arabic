using System.Collections.Generic;
using EA4S.Core;
using EA4S.Database;
using EA4S.Rewards;

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
                        LogManager.I.LogInfo(InfoEvent.AppClosed);
                    }
                    AppManager.I.GameSettings.LastActivePlayerId = value.Id;
                    SaveGameSettings();
                    LogManager.I.LogInfo(InfoEvent.AppStarted);
                    AppManager.I.NavigationManager.SetPlayerNavigationData(currentPlayer);
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
        /// </summary>
        public void ReloadGameSettings()
        {
            AppManager.I.GameSettings = new AppSettings() { AvailablePlayers = new List<string>() { } };
            AppManager.I.GameSettings = AppManager.I.PlayerProfile.LoadGlobalOptions<AppSettings>(new AppSettings()) as AppSettings;
            int lastActivePlayerId = AppManager.I.GameSettings.LastActivePlayerId;
            if (lastActivePlayerId > 0) {
                //CurrentPlayer = LoadPlayerProfileById(AppManager.I.GameSettings.LastActivePlayerId);
                int avatarId = int.Parse(GetAvatarIdFromPlayerId(AppManager.I.GameSettings.LastActivePlayerId));
                SetPlayerProfile(avatarId);
            }
        }

        /// <summary>
        /// Return list of id available for create new player (relate do avatar availble).
        /// </summary>
        /// <returns></returns>
        public List<int> GetListOfUnusedId()
        {
            List<int> returnList = new List<int>();
            for (int i = 1; i < AppConstants.MaxNumberOfPlayerProfiles + 1; i++) {
                if (AppManager.I.GameSettings.AvailablePlayers[i] == null)
                    returnList.Add(i);
            }
            return returnList;
        }

        /// <summary>
        /// Creates the player profile.
        /// </summary>
        /// <param name="age">The age.</param>
        /// <param name="gender">The gender.</param>
        /// <param name="avatarID">The avatar identifier.</param>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        public string CreatePlayerProfile(int age, PlayerGender gender, int avatarID, PlayerTint color) {
            PlayerProfile returnProfile = new PlayerProfile();
            // Data
            returnProfile.Uuid = System.Guid.NewGuid().ToString();
            returnProfile.AvatarId = avatarID;
            // DB Creation
            AppManager.I.DB.CreateDatabaseForPlayer(returnProfile.ToData());
            // Added to list
            AppManager.I.GameSettings.SavedPlayers.Add(returnProfile.GetPlayerIconData());
            // Create new antura skin
            RewardPackUnlockData tileTexture = RewardSystemManager.GetFirstAnturaReward(RewardTypes.texture);
            returnProfile.AddRewardUnlocked(tileTexture);
            returnProfile.CurrentAnturaCustomizations.TileTexture = tileTexture;
            RewardPackUnlockData decalTexture = RewardSystemManager.GetFirstAnturaReward(RewardTypes.decal);
            returnProfile.AddRewardUnlocked(decalTexture);
            returnProfile.CurrentAnturaCustomizations.DecalTexture = decalTexture;
            // Set player profile as current player
            AppManager.I.PlayerProfileManager.CurrentPlayer = returnProfile as PlayerProfile;

            // Call Event Profile creation
            OnNewProfileCreated();

            return returnProfile.Uuid;
        }

        /// <summary>
        /// Sets the player as current player profile by UUID.
        /// </summary>
        /// <param name="playerUUID">The player UUID.</param>
        /// <returns></returns>
        public PlayerProfile SetPlayerAsCurrentByUUID(string playerUUID) {
            PlayerProfile returnProfile = GetPlayerProfileByUUID(playerUUID);
            AppManager.I.PlayerProfileManager.CurrentPlayer = returnProfile;
            return returnProfile;
        }

        /// <summary>
        /// Gets the player profile by UUID.
        /// </summary>
        /// <param name="playerUUID">The player UUID.</param>
        /// <returns></returns>
        public PlayerProfile GetPlayerProfileByUUID(string playerUUID) {
            PlayerProfileData profileFromDB = AppManager.I.DB.LoadDatabaseForPlayer(playerUUID);
            return new PlayerProfile().FromData(profileFromDB);
        }

        /// <summary>
        /// Sets the player profile with corresposnding avatarId to current player.
        /// </summary>
        /// <param name="_avatarId">The avatar identifier.</param>
        /// <param name="_isNew">if set to <c>true</c> create new one.</param>
        /// <returns></returns>
        public PlayerProfile SetPlayerProfile(int _avatarId)
        {
            PlayerProfile returnProfile;
            PlayerProfileData profileFromDB = AppManager.I.DB.LoadDatabaseForPlayer(GetPlayerIdFromAvatarId(_avatarId));

            if (profileFromDB == null) { // not present in db or old db, create new one
                returnProfile = new PlayerProfile();
                // create new
                if (AppManager.I.GameSettings.AvailablePlayers.Contains(_avatarId.ToString())) {
                    returnProfile.Id = AppManager.I.GameSettings.AvailablePlayers.FindIndex(s => s == _avatarId.ToString()) + 1;
                } else {
                    returnProfile.Id = AppManager.I.GameSettings.AvailablePlayers.Count + 1;
                }
                returnProfile.Uuid = System.Guid.NewGuid().ToString();
                returnProfile.AvatarId = _avatarId;
                returnProfile.Key = returnProfile.Id.ToString();
                AppManager.I.DB.CreateDatabaseForPlayer(returnProfile.ToData());
                if (!AppManager.I.GameSettings.AvailablePlayers.Exists(p => p == _avatarId.ToString())) {
                    AppManager.I.GameSettings.AvailablePlayers.Add(_avatarId.ToString());
                    SaveGameSettings();
                }
                // Create new antura skin
                RewardPackUnlockData tileTexture = RewardSystemManager.GetFirstAnturaReward(RewardTypes.texture);
                returnProfile.AddRewardUnlocked(tileTexture);
                returnProfile.CurrentAnturaCustomizations.TileTexture = tileTexture;
                RewardPackUnlockData decalTexture = RewardSystemManager.GetFirstAnturaReward(RewardTypes.decal);
                returnProfile.AddRewardUnlocked(decalTexture);
                returnProfile.CurrentAnturaCustomizations.DecalTexture = decalTexture;
            } else {
                returnProfile = new PlayerProfile().FromData(profileFromDB);
            }
            AppManager.I.PlayerProfileManager.CurrentPlayer = returnProfile as PlayerProfile;

            if (profileFromDB == null && OnNewProfileCreated != null)
                OnNewProfileCreated();

            return AppManager.I.PlayerProfileManager.CurrentPlayer;
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

        public void DeleteCurrentPlayer()
        {
            AppManager.I.Modules.PlayerProfile.DeletePlayer(CurrentPlayer.Key);
        }

        /// <summary>
        /// Gets the player identifier from avatar identifier.
        /// If == 0 player not found.
        /// </summary>
        /// <param name="_avatarId">The avatar identifier.</param>
        /// <returns></returns>
        public int GetPlayerIdFromAvatarId(int _avatarId)
        {
            return AppManager.I.GameSettings.AvailablePlayers.FindIndex(a => a == _avatarId.ToString()) + 1;
        }

        /// <summary>
        /// Gets the avatar identifier from player identifier.
        /// </summary>
        /// <param name="_playerId">The player identifier.</param>
        /// <returns></returns>
        public string GetAvatarIdFromPlayerId(int _playerId)
        {
            return AppManager.I.GameSettings.AvailablePlayers[_playerId - 1];
        }

        /// <summary>
        /// Resets the everything.
        /// </summary>
        public void ResetEverything()
        {
            // Reset all the Databases
            foreach (var playerId in AppManager.I.Modules.PlayerProfile.Options.AvailablePlayers) {
                UnityEngine.Debug.Log(playerId);
                AppManager.I.DB.LoadDatabaseForPlayer(int.Parse(playerId));
                AppManager.I.DB.DropProfile();
            }

            // Reset all profiles (from SRDebugOptions)
            UnityEngine.PlayerPrefs.DeleteAll();
            AppManager.I.PlayerProfileManager.ReloadGameSettings();
            AppManager.I.GameSettings.AvailablePlayers = new System.Collections.Generic.List<string>();
            AppManager.I.PlayerProfileManager.SaveGameSettings();

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