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
using UnityEngine.UI;
using System;
using System.Collections;
using ModularFramework.Core;
using ModularFramework.Modules;

namespace ModularFramework.Components {

    public class PlayersProfileCreate : UIContainer {

        public Text Username;
        public Button CreateButton;

        public override void OnEnable() {
            base.OnEnable();
            // Remove UniRx refactoring request: any reactive interaction within this class must be called manually.
        }

        #region API

        /// <summary>
        /// Create a new profile with data filled in UI form and set newly profile as active.
        /// </summary>
        /// <param name="closeWindow">If true close this window after creation.</param>
        public void CreateNewPlayerProfile(bool closeWindow) {
            IPlayerProfile newPP = GameManager.Instance.PlayerProfile.CreateNewPlayer(new PlayerProfile() {
                Key = Username.text,
            });
            if (closeWindow)
                GameManager.Instance.UIModule.HideUIContainer(Key);
            GameManager.Instance.PlayerProfile.SetActivePlayer<PlayerProfile>(newPP.Key);
        }

        #endregion
    }
}