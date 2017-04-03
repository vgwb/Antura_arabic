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
using ModularFramework.Core;

namespace ModularFramework.Components {
    public class ActualPlayerProfileBox : MonoBehaviour {

        #region UI
        public Text UsernameLable;
        public Text ProgressionLevelLable;
        #endregion

        void OnEnable() {
            // Remove UniRx refactoring request: now OnActivePlayerChanged must be called manually.
        }

        public void OnActivePlayerChanged() {
            if (GameManager.Instance != null && GameManager.Instance.Modules.PlayerProfile.ActivePlayer != null) {
                UsernameLable.text = GameManager.Instance.Modules.PlayerProfile.ActivePlayer.Key;

                Modules.PlayerProfile profile = GameManager.Instance.Modules.PlayerProfile.ActivePlayer as Modules.PlayerProfile;
                ProgressionLevelLable.text = profile.ProgressionRate.ToString();
            }
        }

    }
}
