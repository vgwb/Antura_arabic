// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/12

using System;
using DG.Tweening;
using UnityEngine;

namespace EA4S
{
    public class ProfileSelectorAvatarSelector : MonoBehaviour
    {
        public bool IsShown { get; private set; }
        ProfileSelectorAvatarButton[] avatarButtons;
        Tween showTween;

        #region Unity

        void Awake()
        {
            avatarButtons = this.GetComponentsInChildren<ProfileSelectorAvatarButton>(true);
            for (int i = 0; i < avatarButtons.Length; ++i) avatarButtons[i].SetAvatar(i + 1);

            showTween = this.GetComponent<RectTransform>().DOAnchorPosY(-200, 0.24f).From(true)
                .SetAutoKill(false).Pause()
                .OnPlay(()=> this.gameObject.SetActive(true))
                .OnRewind(()=> this.gameObject.SetActive(false));
            this.gameObject.SetActive(false);

            // Listeners
            foreach (ProfileSelectorAvatarButton bt in avatarButtons) {
                ProfileSelectorAvatarButton b = bt;
                b.Bt.onClick.AddListener(()=> OnClick(b));
            }
        }

        void OnDestroy()
        {
            showTween.Kill();
            foreach (ProfileSelectorAvatarButton bt in avatarButtons) bt.Bt.onClick.RemoveAllListeners();
        }

        #endregion

        #region Public Methods

        internal void Show()
        {
            if (IsShown) return;

            IsShown = true;
            AudioManager.I.PlaySfx(ProfileSelectorUI.I.SfxOpenCreateProfile);

            // Set available avatars
            bool hasProfiles = ProfileSelectorUI.I.ProfileManager.AvailablePlayerProfiles != null
                && ProfileSelectorUI.I.ProfileManager.AvailablePlayerProfiles.Count > 0;
            for (int i = 0; i < avatarButtons.Length; ++i) {
                ProfileSelectorAvatarButton bt = avatarButtons[i];
                int id = i + 1;
                bool isAvailable = true;
                if (hasProfiles) {
                    foreach (PlayerProfile pp in ProfileSelectorUI.I.ProfileManager.AvailablePlayerProfiles) {
                        if (pp.AvatarId == id) {
                            isAvailable = false;
                            break;
                        }
                    }
                }
                bt.SetInteractivity(isAvailable);
            }

            this.gameObject.SetActive(true);
            showTween.PlayForward();
        }

        internal void Hide()
        {
            if (!IsShown) return;

            IsShown = false;
            showTween.PlayBackwards();
            AudioManager.I.PlaySfx(ProfileSelectorUI.I.SfxOpenCreateProfile);
        }

        #endregion

        #region Callbacks

        public void OnClick(ProfileSelectorAvatarButton _bt)
        {
            if (showTween.IsPlaying()) return;

            ProfileSelectorUI.I.AddProfile(Array.IndexOf(avatarButtons, _bt) + 1);
        }

        #endregion
    }
}