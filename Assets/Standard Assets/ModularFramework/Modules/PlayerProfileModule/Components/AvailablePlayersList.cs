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
using UniRx;
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
            // React to change number of element with list refresh
            this.ObserveEveryValueChanged(x => x.PlayersIds.Count).Subscribe(_ => {
                RefreshList();
            });
            // React to change number of element with list refresh
            GameManager.Instance.PlayerProfile.ObserveEveryValueChanged(x => x.ActivePlayer).Subscribe(_ => {
                RefreshList();
            });
            // Refresh click
            if (RefreshListButton)
                RefreshListButton.onClick.AsObservable().Subscribe(_ => {
                    RefreshList();
                }).AddTo(this);
            // Create profile open window click
            if (AddNewButton)
                AddNewButton.onClick.AsObservable().Subscribe(_ => {
                    OpenCreatePlayerProfileWindow();
                }).AddTo(this);
            // Remove button click
            if (RemoveButton)
                RemoveButton.onClick.AsObservable().Subscribe(_ => {
                    // TODO
                }).AddTo(this);
            // Delete all profile button click
            if (DeleteAllProfilesButton)
                DeleteAllProfilesButton.onClick.AsObservable().Subscribe(_ => {
                    DeleteAllProfiles();
                }).AddTo(this);
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