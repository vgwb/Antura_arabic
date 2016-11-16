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
                }
                actualPlayer = value;
            }
        }

        private List<PlayerProfile> availablePlayerProfiles;
        /// <summary>
        /// List of available player profiles (already created).
        /// </summary>
        public List<PlayerProfile> AvailablePlayerProfiles {
            get { return availablePlayerProfiles; }
            set { availablePlayerProfiles = value; }
        }

        #endregion

        #region internal functions
        void reloadGameSettings() {
            AppManager.Instance.GameSettings = new AppSettings() { AvailablePlayers = new List<string>() { } };
            AppManager.Instance.GameSettings = AppManager.Instance.PlayerProfile.LoadGlobalOptions<AppSettings>(new AppSettings()) as AppSettings;
            if(AppManager.Instance.GameSettings.LastActivePlayerId>0)
                ActualPlayer = new PlayerProfile().CreateOrLoadPlayerProfile(AppManager.Instance.GameSettings.LastActivePlayerId == 0 
                    ? "1"
                    : AppManager.Instance.GameSettings.LastActivePlayerId.ToString());
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
            reloadAvailablePlayerProfilesList();
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
            PlayerProfile newPP = new PlayerProfile();
            ActualPlayer = newPP.CreateOrLoadPlayerProfile(_avatarId.ToString());
            return newPP;
        }

        /// <summary>
        /// Saves the game settings.
        /// </summary>
        public void SaveGameSettings() {
            AppManager.Instance.Modules.PlayerProfile.SaveAllOptions();
        }

        /// <summary>
        /// WARNING! Deletes all profiles.
        /// </summary>
        public void DeleteAllProfiles() {
            AppManager.Instance.Modules.PlayerProfile.DeleteAllPlayerProfiles();
        }
        #endregion

    }
}