using System;
using System.Collections;
using DG.DeExtensions;
using DG.Tweening;
using EA4S.Audio;
using EA4S.LivingLetters;
using EA4S.Profile;
using UnityEngine;
using EA4S.Scenes;

namespace EA4S.UI
{
    /// <summary>
    /// General controller for the interface of the Profile Selector.
    /// Used in the Home (_Start) scene.
    /// </summary>
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

        public LetterObjectView LLObjectView;

        #region Unity

        void Awake()
        {
            I = this;

            avatarButtons = ProfilesPanel.GetComponentsInChildren<ProfileSelectorAvatarButton>(true);
            maxProfiles = avatarButtons.Length;

            // By default, the letter shows a truly random letter
            LLObjectView.Initialize(AppManager.I.Teacher.GetRandomTestLetterLL());
        }

        void Start()
        {
            Setup();

            btAddTween = BtAdd.transform.DORotate(new Vector3(0, 0, -45), 0.3f).SetAutoKill(false).Pause()
                .SetEase(Ease.OutBack)
                .OnRewind(() => {
                    if (AppManager.I.GameSettings.AvailablePlayers == null || AppManager.I.GameSettings.AvailablePlayers.Count == 0) BtAdd.Pulse();
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
                AudioManager.I.PlaySound(Sfx.UIButtonClick);
                HomeScene.I.Play();
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
            PlayerProfileManager ppm = AppManager.I.PlayerProfileManager;
            //ppm.SetPlayerProfile(_avatarId);
            ProfileManager.CreatePlayerProfile(/* TODO: pass all data for creation */);
            AudioManager.I.PlaySound(SfxCreateNewProfile);
            LLObjectView.Initialize(AppManager.I.Teacher.GetRandomTestLetterLL());
            Setup();
        }

        /// <summary>
        /// Selects the profile.
        /// </summary>
        /// <param name="_id">Player id.</param>
        internal void SelectProfile(int _id)
        {
            ProfileManager.SetPlayerAsCurrentByUUID(/* TODO: modify to string UUID */);
            AudioManager.I.PlaySound(SfxSelectProfile);
            LLObjectView.Initialize(AppManager.I.Teacher.GetRandomTestLetterLL(useMaxJourneyData: true));
            Setup();
        }

        #endregion

        #region Methods

        // Layout with current profiles
        void Setup()
        {
            ActivateProfileButtons(true);
            int totProfiles = AppManager.I.GameSettings.AvailablePlayers == null ? 0 : AppManager.I.GameSettings.AvailablePlayers.Count;
            int len = avatarButtons.Length;
            for (int i = 0; i < len; ++i) {
                ProfileSelectorAvatarButton bt = GetAvatarButtonByPlayerId(i + 1); // right to left
                if (i >= totProfiles) bt.gameObject.SetActive(false);
                else {
                    bt.gameObject.SetActive(true);
                    bt.SetAvatar(int.Parse(AppManager.I.GameSettings.AvailablePlayers[i]));
                    // PLAYER REFACTORING WITH UUID
                    //if (i == AppManager.I.Player.Id - 1) bt.Toggle(true, true);
                    //else bt.Toggle(false);
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
            // PLAYER REFACTORING WITH UUID
            //BtPlay.RectT.SetAnchoredPosX(GetAvatarButtonByPlayerId(AppManager.I.Player.Id).RectT.anchoredPosition.x);
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
                    if (AppManager.I.GameSettings.AvailablePlayers != null && AppManager.I.GameSettings.AvailablePlayers.Count > 0) btPlayTween.PlayForward();
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