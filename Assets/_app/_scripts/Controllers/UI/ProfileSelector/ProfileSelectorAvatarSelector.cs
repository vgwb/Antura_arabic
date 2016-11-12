// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/12

using DG.Tweening;
using UnityEngine;

namespace EA4S
{
    public class ProfileSelectorAvatarSelector : MonoBehaviour
    {
        public bool IsShown { get; private set; }
        ProfileSelectorAvatarButton[] avatarButtons;
        Sequence showTween;

        #region Unity

        void Awake()
        {
            avatarButtons = this.GetComponentsInChildren<ProfileSelectorAvatarButton>(true);

            showTween = DOTween.Sequence().SetAutoKill(false).Pause()
                .OnRewind(()=> this.gameObject.SetActive(false));
            for (int i = 0; i < avatarButtons.Length; ++i) {
                ProfileSelectorAvatarButton bt = avatarButtons[i];
                showTween.Insert(i * 0.05f, bt.transform.DOScale(0.0001f, 0.3f).From().SetEase(Ease.OutBack));
            }
        }

        void OnDestroy()
        {
            showTween.Kill();
        }

        #endregion

        #region Public Methods

        internal void Show()
        {
            if (IsShown) return;

            IsShown = true;
            this.gameObject.SetActive(true);
            showTween.PlayForward();
        }

        internal void Hide()
        {
            if (!IsShown) return;

            IsShown = false;
            showTween.PlayBackwards();
        }

        #endregion
    }
}