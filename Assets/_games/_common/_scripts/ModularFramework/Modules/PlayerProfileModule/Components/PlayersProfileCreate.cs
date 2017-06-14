
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using EA4S.Profile;

namespace EA4S.Core
{
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
            IPlayerProfile newPP = EA4S.AppManager.Instance.PlayerProfile.CreateNewPlayer(new PlayerProfile() {
                Key = Username.text,
            });
            if (closeWindow)
                EA4S.AppManager.Instance.UIModule.HideUIContainer(Key);
            EA4S.AppManager.Instance.PlayerProfile.SetActivePlayer<PlayerProfile>(newPP.Key);
        }

        #endregion
    }
}