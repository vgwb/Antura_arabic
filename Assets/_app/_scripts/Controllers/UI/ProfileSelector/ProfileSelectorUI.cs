// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/12

using UnityEngine;

namespace EA4S
{
    public class ProfileSelectorUI : MonoBehaviour
    {
        [Header("References")]
        public UIButton BtAdd;
        public UIButton BtPlay;
        public GameObject ProfilesPanel;
        public ProfileSelectorAvatarSelector AvatarSelector;

        public static ProfileSelectorUI I;
        int maxProfiles;
        ProfileSelectorAvatarButton[] avatarButtons;

        #region Unity

        void Awake()
        {
            I = this;

            avatarButtons = ProfilesPanel.GetComponentsInChildren<ProfileSelectorAvatarButton>(true);
            maxProfiles = avatarButtons.Length;
        }

        void OnDestroy()
        {
            if (I == this) I = null;
        }

        #endregion
    }
}