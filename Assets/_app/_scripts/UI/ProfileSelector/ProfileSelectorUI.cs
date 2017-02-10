using System;
using System.Collections;
using System.Collections.Generic;
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
        [Header("Audio")]
        public Sfx SfxOpenCreateProfile;
        public Sfx SfxCreateNewProfile;
        public Sfx SfxSelectProfile;

        public PlayerProfileManager ProfileManager { get { return AppManager.I.PlayerProfileManager; } }
        int maxProfiles;
        List<PlayerIconData> playerIconDatas;
        PlayerIcon[] playerIcons;
        Tween btAddTween, btPlayTween;

        public LetterObjectView LLObjectView; // ?

        #region Unity

        void Awake()
        {
            playerIcons = ProfilesPanel.GetComponentsInChildren<PlayerIcon>(true);
            maxProfiles = playerIcons.Length;

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
            foreach (PlayerIcon pIcon in playerIcons) {
                PlayerIcon p = pIcon;
                p.UIButton.Bt.onClick.AddListener(() => OnSelectProfile(p));
            }
        }

        void OnDestroy()
        {
            btAddTween.Kill();
            btPlayTween.Kill();
            BtAdd.Bt.onClick.RemoveAllListeners();
            foreach (PlayerIcon pIcon in playerIcons) pIcon.UIButton.Bt.onClick.RemoveAllListeners();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Selects the profile.
        /// </summary>
        /// <param name="uuid">Player UUID.</param>
        internal void SelectProfile(string uuid)
        {
            // PLAYER REFACTORING WITH UUID
            // ProfileManager.SetPlayerAsCurrentByUUID(/* TODO: modify to string UUID */);
            AudioManager.I.PlaySound(SfxSelectProfile);
            LLObjectView.Initialize(AppManager.I.Teacher.GetRandomTestLetterLL(useMaxJourneyData: true));
            Setup();
        }

        #endregion

        #region Methods

        // Layout with current profiles
        void Setup()
        {
            ActivatePlayerIcons(true);
            if (playerIconDatas == null) playerIconDatas = ProfileManager.GetSavedPlayers();
            int totProfiles = playerIconDatas == null ? 0 : playerIconDatas.Count;
            int len = playerIcons.Length;
            for (int i = 0; i < len; ++i)
            {
                PlayerIcon playerIcon = playerIcons[i];
                if (i >= totProfiles) playerIcon.gameObject.SetActive(false);
                else {
                    PlayerIconData data = playerIconDatas[i];
                    playerIcon.gameObject.SetActive(true);
                    playerIcon.Init(data);
                    // PLAYER REFACTORING WITH UUID (test for now)
//                    if (i == AppManager.I.Player.Id - 1) playerIcon.Select();
//                    else playerIcon.Deselect();
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

        void ActivatePlayerIcons(bool _activate)
        {
            foreach (PlayerIcon pIcon in playerIcons) pIcon.UIButton.Bt.interactable = _activate;
        }

        #endregion

        #region Callbacks

        void OnClick(UIButton _bt)
        {
            if (_bt == BtAdd) {
                // Bt Add
                _bt.StopPulsing();
                // PLAYER REFACTORING WITH UUID
                AppManager.I.NavigationManager.GotoNewProfileCreation();
            }
        }

        void OnSelectProfile(PlayerIcon playerIcon)
        {
            int index = Array.IndexOf(playerIcons, playerIcon);
            PlayerIconData playerData = playerIconDatas[index];
            SelectProfile(playerData.Uuid);
        }

        #endregion
    }
}