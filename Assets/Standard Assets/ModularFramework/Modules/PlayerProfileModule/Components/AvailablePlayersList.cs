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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModularFramework.Core;
using ModularFramework.Modules;

namespace ModularFramework.Components {
    /// <summary>
    /// Component for manage users profiles.
    /// If Buttons public variables are not null onclick event will be auto wired for execute correct function.
    /// </summary>
    [AddComponentMenu("Modular Framework/PlayerProfile Module/AvailablePlayersList")]
    public class AvailablePlayersList : UIContainer {

        #region Graphic Elements
        public Transform ListContainer;
        public GameObject SingleItemPrefab;
        /// <summary>
        /// Perform refresh of the profile list.
        /// </summary>
        public Button RefreshListButton;
        /// <summary>
        /// Open the "add new profile" window.
        /// </summary>
        public Button AddNewButton;
        /// <summary>
        /// TODO: Remove player profile.
        /// </summary>
        public Button RemoveButton;
        /// <summary>
        /// Delete all 
        /// </summary>
        public Button DeleteAllProfilesButton;
        #endregion

        public List<string> PlayersIds;

        public override void OnEnable() {
            base.OnEnable();
            // Remove UniRx refactoring request: any reactive interaction within this class must be called manually.
        }

        #region component API
        /// <summary>
        /// Open creation profile windows.
        /// </summary>
        public void OpenCreatePlayerProfileWindow() {
            GameManager.Instance.UIModule.ShowUIContainer("NewProfileWindow");
        }

        /// <summary>
        /// Retrive list of available players and visual refresh list.
        /// </summary>
        public void RefreshList() {
            ClearList();
            GetPlayers();
            foreach (var p in PlayersIds) {
                PlayerProfileSetActive newComponent = Instantiate(SingleItemPrefab).GetComponent<PlayerProfileSetActive>();
                newComponent.transform.SetParent(ListContainer);
                newComponent.Init(GameManager.Instance.PlayerProfile.LoadPlayerSettings<PlayerProfile>(p));
                newComponent.gameObject.name = newComponent.Player.Key;
            }
        }

        /// <summary>
        /// Clear profile list from all visual items.
        /// </summary>
        public void ClearList() {
            foreach (var item in ListContainer.GetComponentsInChildren<PlayerProfileSetActive>()) {
                Destroy(item.gameObject);
            } 
        }

        /// <summary>
        /// Delete all available profile, clear list and set actual profile to null.
        /// </summary>
        public void DeleteAllProfiles() {
            GameManager.Instance.PlayerProfile.DeleteAllPlayerProfiles();
            ClearList();
        }

        #endregion

        void GetPlayers() {
            //PlayersIds = GameManager.Instance.PlayerProfile.LoadGlobalOptions().AvailablePlayers;
        }
    }
}