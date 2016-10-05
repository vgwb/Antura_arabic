/* --------------------------------------------------------------
*   Indie Contruction : Modular Framework for Unity
*   Copyright(c) 2016 Indie Construction / Paolo Bragonzi
*   All rights reserved. 
*   For any information refer to http://www.indieconstruction.com
*   
*   This library is free software; you can redistribute it and/or
*   modify it under the terms of the GNU Lesser General Public
*   License as published by the Free Software Foundation; either
*   version 3.0 of the License, or(at your option) any later version.
*   
*   This library is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
*   Lesser General Public License for more details.
*   
*   You should have received a copy of the GNU Lesser General Public
*   License along with this library.
* -------------------------------------------------------------- */
using UnityEngine;
using System;
using System.Collections.Generic;
using ModularFramework.Core;

namespace ModularFramework.Modules
{
    /// <summary>
    /// Concrete implementation for module type PlayerProfileModule.
    /// </summary>
    public class PlayerProfileModuleDefault : IPlayerProfileModule
    {
        public IPlayerProfile ActivePlayer { get; set; }

        #region IModule Implementation
        public IPlayerProfileModule ConcreteModuleImplementation { get; set; }
        private PlayersData players;
        public PlayersData Players {
            get {
                if (players == null)
                    players = LoadAllPlayerProfiles();
                return players;
            }
            set { players = value; }
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
        public IPlayerProfile CreateNewPlayer(IPlayerProfile _newPlayer, IPlayerExtendedProfile _extProfile = null) {
            if (!Players.AvailablePlayers.Exists(p => p == _newPlayer.Id)) {
                Players.AvailablePlayers.Add(_newPlayer.Id);
                SavePlayerSettings(_newPlayer);
                SaveAllPlayerProfiles();
            }
            return _newPlayer;
        }

        /// <summary>
        /// Delete player.
        /// </summary>
        /// <param name="_playerId"></param>
        public void DeletePlayer(string _playerId) {
            Players.AvailablePlayers.Remove(_playerId);
        }

        /// <summary>
        /// Load player settings.
        /// </summary>
        /// <param name="_playerId"></param>
        public IPlayerProfile LoadPlayerSettings<T>(string _playerId) where T : IPlayerProfile {
            if (PlayerPrefs.HasKey(GetStoreKeyForPlayer(_playerId))) {
                string serializableProfile = PlayerPrefs.GetString(GetStoreKeyForPlayer(_playerId));
                IPlayerProfile returnProfile = JsonUtility.FromJson<T>(serializableProfile);
                returnProfile.Id = _playerId;
                return returnProfile;
            } else {
                Debug.LogFormat("Profile {0} not found.", _playerId);
            }
            return null;
        }

        /// <summary>
        /// Save player settings on PlayerPrefs (do not update list of players).
        /// </summary>
        /// <param name="_newPlayer"></param>
        /// <param name="_extProfile"></param>
        public void SavePlayerSettings(IPlayerProfile _newPlayer, IPlayerExtendedProfile _extProfile = null)
        {
            string storeKey = GetStoreKeyForPlayer(_newPlayer.Id);
            string serializedObjs = JsonUtility.ToJson(_newPlayer);
            if (serializedObjs != null)
                PlayerPrefs.SetString(storeKey, serializedObjs);
            else
                Debug.Log("Unable to serialize player profile : " + _newPlayer.Id);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Set the active player.
        /// </summary>
        /// <param name="_playerId"></param>
        public void SetActivePlayer<T>(string _playerId) where T : IPlayerProfile {
            if (!Players.AvailablePlayers.Exists(p => p == _playerId)) // If player is not in active players list...
                return;
            IPlayerProfile pp = LoadPlayerSettings<T>(_playerId);
            if (pp == null) {
                Debug.LogError("Player not found");
            } else {
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

        /// <summary>
        /// Return the list of all availables player profiles.
        /// </summary>
        /// <returns></returns>
        public PlayersData LoadAllPlayerProfiles()
        {
            string serializedObjs;
            if (PlayerPrefs.HasKey(PLAYERS_PREFS_KEY)) {
                serializedObjs = PlayerPrefs.GetString(PLAYERS_PREFS_KEY);
                return JsonUtility.FromJson<PlayersData>(serializedObjs);
            } else {
                // Players list not created yet.
                return new PlayersData();
            }
        }

        /// <summary>
        /// Save all player profiles.
        /// </summary>
        public void SaveAllPlayerProfiles() {
            string serializedObjs = JsonUtility.ToJson(Players);
            PlayerPrefs.SetString(PLAYERS_PREFS_KEY, serializedObjs);
            PlayerPrefs.Save();
        }

        #region Data Store
        const string PLAYERS_PREFS_KEY = "PLAYERS";
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
    }
}
