// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/12

using System.Collections.Generic;
using DG.DeExtensions;
using DG.Tweening;
using UnityEngine;

namespace EA4S
{
    public class ProfileSelectorUI : MonoBehaviour
    {
        public bool UseFakeDebugData;
        [Header("References")]
        public UIButton BtAdd;
        public UIButton BtPlay;
        public GameObject ProfilesPanel;
        public ProfileSelectorAvatarSelector AvatarSelector;

        public static ProfileSelectorUI I;
        public const string AvatarsResourcesDir = "Images/Avatars/";
        PlayerProfileManager profileManager { get { return null; /*TODO*/ } }
        int maxProfiles;
        List<PlayerProfile> playerProfiles;
        ProfileSelectorAvatarButton[] avatarButtons;
        Tween btAddTween;

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
                .SetEase(Ease.OutBack);

            // Listeners
            BtAdd.Bt.onClick.AddListener(()=> OnClick(BtAdd));
        }

        void OnDestroy()
        {
            if (I == this) I = null;
            btAddTween.Kill();
        }

        #endregion

        #region Public Methods

        internal void AddProfile(int _avatarId)
        {
            if (UseFakeDebugData) {
                if (playerProfiles == null) playerProfiles = new List<PlayerProfile>();
                PlayerProfile pp = new PlayerProfile();
                pp.Id = playerProfiles.Count;
                pp.AvatarId = _avatarId;
                playerProfiles.Add(pp);
            }

            Setup();
        }

        #endregion

        #region Methods

        // Layout with current profiles
        void Setup()
        {
            if (!UseFakeDebugData) playerProfiles = profileManager.AvailablePlayerProfiles;

            int totProfiles = playerProfiles == null ? 0 : playerProfiles.Count;
            for (int i = 0; i < avatarButtons.Length; ++i) {
                if (i >= totProfiles) avatarButtons[i].gameObject.SetActive(false);
                else {
                    // TODO
                }
            }

            if (totProfiles == 0) {
                BtAdd.Pulse();
                BtPlay.gameObject.SetActive(false);
            } else {
//                BtPlay.RectT.SetAnchoredPosX(avatarButtons[player]);
            }
            if (totProfiles >= maxProfiles) {
                btAddTween.Rewind();
                BtAdd.gameObject.SetActive(false);
            }
        }

        #endregion

        #region Callbacks

        void OnClick(UIButton _bt)
        {
            if (_bt == BtAdd) {
                _bt.StopPulsing();
                if (AvatarSelector.IsShown) {
                    btAddTween.PlayBackwards();
                    AvatarSelector.Hide();
                } else {
                    btAddTween.PlayForward();
                    AvatarSelector.Show();
                }
            } else {
                _bt.AnimateClick();
            }
        }

        #endregion
    }
}