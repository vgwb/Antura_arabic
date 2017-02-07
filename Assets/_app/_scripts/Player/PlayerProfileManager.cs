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

        /// <summary>
        /// Max number of player profiles usable.
        /// </summary>
        public int MaxNumberOfPlayerProfiles = 5;

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
                    if (OnProfileChanged != null)
                        OnProfileChanged();
                }
                currentPlayer = value;
                

            }
        }

        // TODO : Refactor Reward System

        //private List<PlayerProfile> availablePlayerProfiles;
        ///// <summary>
        ///// List of available player profiles (already created).
        ///// </summary>
        //public List<PlayerProfile> AvailablePlayerProfiles {
        //    get {
        //        reloadAvailablePlayerProfilesList();
        //        return availablePlayerProfiles;
        //    }
        //    set { availablePlayerProfiles = value; }
        //}

        #endregion

        #region internal functions
        

        // TODO : Refactor Reward System
        //void reloadAvailablePlayerProfilesList() {
        //    List<PlayerProfile> returnList = new List<PlayerProfile>();
        //    foreach (string pId in AppManager.I.GameSettings.AvailablePlayers) {

        //        PlayerProfile pp = AppManager.I.Modules.PlayerProfile.LoadPlayerSettings<PlayerProfile>(pId) as PlayerProfile;
        //        if (pp != null)
        //            returnList.Add(pp);
        //    }
        //    availablePlayerProfiles = returnList;
        //}
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
        public void ReloadGameSettings() {
            AppManager.I.GameSettings = new AppSettings() { AvailablePlayers = new List<string>() { } };
            AppManager.I.GameSettings = AppManager.I.PlayerProfile.LoadGlobalOptions<AppSettings>(new AppSettings()) as AppSettings;
            int lastActivePlayerId = AppManager.I.GameSettings.LastActivePlayerId;
            if (lastActivePlayerId > 0) {
                CurrentPlayer = LoadPlayerProfileById(AppManager.I.GameSettings.LastActivePlayerId);
            }
            // TODO : Refactor Reward System
            //reloadAvailablePlayerProfilesList();
        }

        /// <summary>
        /// Return list of id available for create new player (relate do avatar availble).
        /// </summary>
        /// <returns></returns>
        public List<int> GetListOfUnusedId()
        {
            List<int> returnList = new List<int>();
            for (int i = 1; i < MaxNumberOfPlayerProfiles + 1; i++) {
                if (AppManager.I.GameSettings.AvailablePlayers[i] == null)
                    returnList.Add(i);
            }
            return returnList;
        }

        /// <summary>
        /// Sets the player profile with corresposnding avatarId to current player.
        /// </summary>
        /// <param name="_avatarId">The avatar identifier.</param>
        /// <param name="_isNew">if set to <c>true</c> create new one.</param>
        /// <returns></returns>
        public PlayerProfile SetPlayerProfile(int _avatarId, bool _isNew)
        {
            PlayerProfile returnProfile;
            if (_isNew) {
                returnProfile = new PlayerProfile();
                // create new
                returnProfile.Id = AppManager.I.GameSettings.AvailablePlayers.Count + 1;
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
                // TODO : Refactor Reward System
                // returnProfile = AppManager.I.Modules.PlayerProfile.CreateNewPlayer(returnProfile) as PlayerProfile;
            } else {
                returnProfile = LoadPlayerProfileByAvatarId(_avatarId);
            }
            AppManager.I.PlayerProfileManager.CurrentPlayer = returnProfile as PlayerProfile;
            // -----
            // TODO : Refactor Reward System
            //AppManager.I.PlayerProfileManager.availablePlayerProfiles.Add(AppManager.I.PlayerProfileManager.CurrentPlayer);
            //AppManager.I.PlayerProfileManager.CurrentPlayer.Save();
            if (_isNew && OnNewProfileCreated != null)
                OnNewProfileCreated();

            return AppManager.I.PlayerProfileManager.CurrentPlayer;
        }

        /// <summary>
        /// Saves the player settings.
        /// </summary>
        /// <param name="_playerProfile">The player profile.</param>
        public void SavePlayerSettings(PlayerProfile _playerProfile)
        {
            // TODO : Refactor Reward System
            AppManager.I.DB.UpdatePlayerProfileData(_playerProfile.ToData());
            // Old 
            //AppManager.I.Modules.PlayerProfile.SavePlayerSettings(_playerProfile);
        }

        /// <summary>
        /// Loads the player profile by avatar identifier.
        /// </summary>
        /// <param name="_avatarId">The avatar identifier.</param>
        /// <returns></returns>
        public PlayerProfile LoadPlayerProfileByAvatarId(int _avatarId)
        {
            return LoadPlayerProfileById(GetPlayerIdFromAvatarId(_avatarId));
        }

        /// <summary>
        /// Loads the player profile by identifier.
        /// </summary>
        /// <param name="_Id">The identifier.</param>
        /// <returns></returns>
        public PlayerProfile LoadPlayerProfileById(int _Id)
        {
            // TODO : Refactor Reward System
            return new PlayerProfile().FromData(AppManager.I.DB.LoadDatabaseForPlayer(_Id));
                
            // return AppManager.I.PlayerProfile.LoadPlayerSettings<PlayerProfile>(_Id.ToString()) as PlayerProfile;
        }

        /// <summary>
        /// Saves the game settings.
        /// </summary>
        public void SaveGameSettings()
        {
            AppManager.I.Modules.PlayerProfile.Options = AppManager.I.GameSettings;
            AppManager.I.Modules.PlayerProfile.SaveAllOptions();
            // TODO : Refactor Reward System
            //reloadAvailablePlayerProfilesList();
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
        public void ResetEverything() {
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