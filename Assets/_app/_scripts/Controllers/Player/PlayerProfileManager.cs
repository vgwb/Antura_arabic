using UnityEngine;
using System.Collections.Generic;

namespace EA4S {
    /// <summary>
    /// Player profile manager.
    /// </summary>
    public class PlayerProfileManager {

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
                if (actualPlayer != value)
                    AppManager.Instance.Player = value;
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
            //newPP.CreateOrLoadPlayerProfile(_avatarId);
            return newPP;
        }


    }
}