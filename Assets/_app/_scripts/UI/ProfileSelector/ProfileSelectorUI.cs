// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/12

using System;
using System.Collections;
using System.Collections.Generic;
using DG.DeExtensions;
using DG.Tweening;
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
        [Header("Audio")]
        public Sfx SfxOpenCreateProfile;
        public Sfx SfxCreateNewProfile;
        public Sfx SfxSelectProfile;

        public static ProfileSelectorUI I;
        public PlayerProfileManager ProfileManager { get { return AppManager.I.PlayerProfileManager; } }
        int maxProfiles;
        ProfileSelectorAvatarButton[] avatarButtons;
        Tween btAddTween, btPlayTween;

        #region Unity

        void Awake()
        {
            I = this;

            avatarButtons = ProfilesPanel.GetComponentsInChildren<ProfileSelectorAvatarButton>(true);
            maxProfiles = avatarButtons.Length;
        }

        void Start()
        {
            Setup();

            btAddTween = BtAdd.transform.DORotate(new Vector3(0, 0, -45), 0.3f).SetAutoKill(false).Pause()
                .SetEase(Ease.OutBack)
                .OnRewind(() => {
                    if (ProfileManager.AvailablePlayerProfiles == null || ProfileManager.AvailablePlayerProfiles.Count == 0) BtAdd.Pulse();
                });
            btPlayTween = DOTween.Sequence().SetAutoKill(false).Pause()
                .Append(BtPlay.RectT.DOAnchorPosY(-210, 0.2f).From(true))
                .OnPlay(() => BtPlay.gameObject.SetActive(true))
                .OnRewind(() => BtPlay.gameObject.SetActive(false))
                .OnComplete(() => BtPlay.Pulse());

            BtPlay.gameObject.SetActive(false);

            // Listeners
            BtAdd.Bt.onClick.AddListener(() => OnClick(BtAdd));
            BtPlay.Bt.onClick.AddListener(() => {
                AudioManager.I.PlaySfx(Sfx.UIButtonClick);
                HomeManager.I.Play();
            });
            foreach (ProfileSelectorAvatarButton bt in avatarButtons) {
                ProfileSelectorAvatarButton b = bt;
                b.Bt.onClick.AddListener(() => OnClick(b));
            }
        }

        void OnDestroy()
        {
            if (I == this) I = null;
            btAddTween.Kill();
            btPlayTween.Kill();
            BtAdd.Bt.onClick.RemoveAllListeners();
            foreach (ProfileSelectorAvatarButton bt in avatarButtons) bt.Bt.onClick.RemoveAllListeners();
        }

        #endregion

        #region Public Methods

        internal void AddProfile(int _avatarId)
        {
            AvatarSelector.Hide();
            btAddTween.PlayBackwards();

            PlayerProfile pp = ProfileManager.CreateOrLoadPlayerProfile(_avatarId);
            ProfileManager.CurrentPlayer = pp;
            AudioManager.I.PlaySfx(SfxCreateNewProfile);

            Setup();
        }

        internal void SelectProfile(int _id)
        {
            ProfileManager.CurrentPlayer = ProfileManager.AvailablePlayerProfiles[_id - 1];
            AudioManager.I.PlaySfx(SfxSelectProfile);
            Setup();
        }

        #endregion

        #region Methods

        // Layout with current profiles
        void Setup()
        {
            ActivateProfileButtons(true);
            int totProfiles = ProfileManager.AvailablePlayerProfiles == null ? 0 : ProfileManager.AvailablePlayerProfiles.Count;
            int len = avatarButtons.Length;
            for (int i = 0; i < len; ++i) {
                ProfileSelectorAvatarButton bt = GetAvatarButtonByPlayerId(i + 1); // right to left
                if (i >= totProfiles) bt.gameObject.SetActive(false);
                else {
                    bt.gameObject.SetActive(true);
                    bt.SetAvatar(ProfileManager.AvailablePlayerProfiles[i].AvatarId);
                    if (i == ProfileManager.CurrentPlayer.Id - 1) bt.Toggle(true, true);
                    else bt.Toggle(false);
                }
            }

            if (totProfiles == 0) {
                BtAdd.Pulse();
                BtPlay.StopPulsing();
                btPlayTween.PlayBackwards();
            } else {
                // Set play button position
                this.StartCoroutine(CO_SetupPlayButton());
            }
            if (totProfiles >= maxProfiles) {
                btAddTween.Rewind();
                BtAdd.gameObject.SetActive(false);
            }
        }

        // Used to set play button position after one frame, so grid is set correctly
        IEnumerator CO_SetupPlayButton()
        {
            yield return null;

            BtPlay.gameObject.SetActive(true);
            BtPlay.RectT.SetAnchoredPosX(GetAvatarButtonByPlayerId(ProfileManager.CurrentPlayer.Id).RectT.anchoredPosition.x);
            btPlayTween.PlayForward();
        }

        void ActivateProfileButtons(bool _activate)
        {
            foreach (ProfileSelectorAvatarButton bt in avatarButtons) bt.SetInteractivity(_activate);
        }

        #endregion

        #region Callbacks

        void OnClick(UIButton _bt)
        {
            if (_bt == BtAdd) {
                // Bt Add
                _bt.StopPulsing();
                if (AvatarSelector.IsShown) {
                    btAddTween.PlayBackwards();
                    AvatarSelector.Hide();
                    if (ProfileManager.AvailablePlayerProfiles != null && ProfileManager.AvailablePlayerProfiles.Count > 0) btPlayTween.PlayForward();
                    ActivateProfileButtons(true);
                } else {
                    btAddTween.PlayForward();
                    AvatarSelector.Show();
                    BtPlay.StopPulsing();
                    btPlayTween.PlayBackwards();
                    ActivateProfileButtons(false);
                }
            } else if (!btAddTween.IsPlaying()) {
                // Profile button
                SelectProfile(GetPlayerIdByAvatarButton(_bt as ProfileSelectorAvatarButton));
            }
        }

        #endregion

        #region Helpers

        ProfileSelectorAvatarButton GetAvatarButtonByPlayerId(int _playerId)
        {
            return avatarButtons[avatarButtons.Length - _playerId];
        }

        int GetPlayerIdByAvatarButton(ProfileSelectorAvatarButton _bt)
        {
            return avatarButtons.Length - Array.IndexOf(avatarButtons, _bt);
        }

        #endregion
    }
}