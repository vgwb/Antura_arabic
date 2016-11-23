using UnityEngine;
using System.Collections.Generic;

namespace EA4S {
    /// <summary>
    /// Player profile manager.
    /// </summary>
    public class PlayerProfileManager {

        #region Properties

        /// <summary>
        /// Max number of player profiles usables.
        /// </summary>
        public int MaxNumberOfPlayerProfiles = 5;

        private PlayerProfile actualPlayer;
        /// <summary>
        /// Actual Player.
        /// </summary>
        public PlayerProfile ActualPlayer {
            get { return actualPlayer; }
            set {
                if (actualPlayer != value) {
                    AppManager.Instance.Player = value;
                    AppManager.Instance.GameSettings.LastActivePlayerId = value.Id;
                    SaveGameSettings();
                    AppManager.Instance.InitDataAI();
                }
                actualPlayer = value;
                
            }
        }

        private List<PlayerProfile> availablePlayerProfiles;
        /// <summary>
        /// List of available player profiles (already created).
        /// </summary>
        public List<PlayerProfile> AvailablePlayerProfiles {
            get {
                reloadAvailablePlayerProfilesList();
                return availablePlayerProfiles; }
            set { availablePlayerProfiles = value; }
        }

        #endregion

        #region internal functions
        void reloadGameSettings() {
            AppManager.Instance.GameSettings = new AppSettings() { AvailablePlayers = new List<string>() { } };
            AppManager.Instance.GameSettings = AppManager.Instance.PlayerProfile.LoadGlobalOptions<AppSettings>(new AppSettings()) as AppSettings;
            if (AppManager.Instance.GameSettings.LastActivePlayerId > 0)
                ActualPlayer = LoadPlayerProfileById(AppManager.Instance.GameSettings.LastActivePlayerId);
            reloadAvailablePlayerProfilesList();
        }

        void reloadAvailablePlayerProfilesList() {
            List<PlayerProfile> returnList = new List<PlayerProfile>();
            foreach (string pId in AppManager.Instance.GameSettings.AvailablePlayers) {
                PlayerProfile pp = AppManager.Instance.Modules.PlayerProfile.LoadPlayerSettings<PlayerProfile>(pId) as PlayerProfile;
                if (pp != null)
                    returnList.Add(pp);
            }
            availablePlayerProfiles = returnList;
        }
        #endregion

        #region API        
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerProfileManager"/> class.
        /// </summary>
        public PlayerProfileManager() {
            reloadGameSettings();
        }

        /// <summary>
        /// Return list of id available for create new player (relate do avatar availble).
        /// </summary>
        /// <returns></returns>
        public List<int> GetListOfUnusedId() {
            List<int> returnList = new List<int>();
            for (int i = 1; i < MaxNumberOfPlayerProfiles + 1; i++) {
                if (availablePlayerProfiles.Find(p => p.Id == i) == null)
                    returnList.Add(i);
            }
            return returnList;
        }

        /// <summary>
        /// Return PlayerProfile with avatar id in param.
        /// If not exist create new with default settings.
        /// </summary>
        /// <param name="_avatarId"></param>
        /// <returns></returns>
        public PlayerProfile CreateOrLoadPlayerProfile(int _avatarId) {

            PlayerProfile returnProfile = LoadPlayerProfileByAvatarId(_avatarId);
            if (returnProfile == null) {
                returnProfile = new PlayerProfile();
                // create new
                returnProfile.Id = AvailablePlayerProfiles.Count + 1;
                returnProfile.AvatarId = _avatarId;
                returnProfile.Key = returnProfile.Id.ToString();
                returnProfile = AppManager.Instance.Modules.PlayerProfile.CreateNewPlayer(returnProfile) as PlayerProfile;
            }
            AppManager.Instance.PlayerProfileManager.ActualPlayer = returnProfile as PlayerProfile;
            AppManager.Instance.PlayerProfileManager.availablePlayerProfiles.Add(AppManager.Instance.PlayerProfileManager.ActualPlayer);
            //AppManager.Instance.GameSettings.AvailablePlayers.Add(AppManager.Instance.PlayerProfileManager.ActualPlayer.Id.ToString());
            SaveGameSettings();
            return AppManager.Instance.PlayerProfileManager.ActualPlayer;
        }

        /// <summary>
        /// Saves the player settings.
        /// </summary>
        /// <param name="_playerProfile">The player profile.</param>
        public void SavePlayerSettings(PlayerProfile _playerProfile) {
            AppManager.Instance.Modules.PlayerProfile.SavePlayerSettings(_playerProfile);
        }

        /// <summary>
        /// Loads the player profile by avatar identifier.
        /// </summary>
        /// <param name="_avatarId">The avatar identifier.</param>
        /// <returns></returns>
        public PlayerProfile LoadPlayerProfileByAvatarId(int _avatarId) {
            return LoadPlayerProfileById(GetPlayerIdFromAvatarId(_avatarId));
        }

        /// <summary>
        /// Loads the player profile by identifier.
        /// </summary>
        /// <param name="_Id">The identifier.</param>
        /// <returns></returns>
        public PlayerProfile LoadPlayerProfileById(int _Id) {
            return AppManager.Instance.PlayerProfile.LoadPlayerSettings<PlayerProfile>(_Id.ToString()) as PlayerProfile;
        }

        /// <summary>
        /// Saves the game settings.
        /// </summary>
        public void SaveGameSettings() {
            AppManager.Instance.Modules.PlayerProfile.Options = AppManager.Instance.GameSettings;
            AppManager.Instance.Modules.PlayerProfile.SaveAllOptions();
            reloadAvailablePlayerProfilesList();
        }

        /// <summary>
        /// WARNING! Deletes all profiles.
        /// </summary>
        public void DeleteAllProfiles() {
            AppManager.Instance.Modules.PlayerProfile.DeleteAllPlayerProfiles();
        }

        /// <summary>
        /// Gets the player identifier from avatar identifier.
        /// </summary>
        /// <param name="_avatarId">The avatar identifier.</param>
        /// <returns></returns>
        public int GetPlayerIdFromAvatarId(int _avatarId) {
            PlayerProfile pp = availablePlayerProfiles.Find(p => p.AvatarId == _avatarId);
            if (pp != null)
                return pp.Id;
            else
                return 0;
        }
        #endregion

    }
}