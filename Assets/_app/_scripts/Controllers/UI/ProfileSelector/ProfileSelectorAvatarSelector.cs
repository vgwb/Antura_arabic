// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/12

using UnityEngine;

namespace EA4S
{
    public class ProfileSelectorAvatarSelector : MonoBehaviour
    {
        public bool IsShown { get; private set; }
        ProfileSelectorAvatarButton[] avatarButtons;

        #region Unity

        void Awake()
        {
            avatarButtons = this.GetComponentsInChildren<ProfileSelectorAvatarButton>(true);
        }

        #endregion

        #region Public Methods

        internal void Show()
        {
            if (IsShown) return;

            IsShown = true;
        }

        internal void Hide()
        {
            if (!IsShown) return;

            IsShown = false;
        }

        #endregion
    }
}