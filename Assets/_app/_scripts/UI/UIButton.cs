// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/12

using DG.DeExtensions;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EA4S
{
    [RequireComponent(typeof(Button))]
    public class UIButton : MonoBehaviour
    {
        public Color BtToggleOffColor = Color.white;
        public Color BtLockedColor = Color.red;
        public bool ToggleIconAlpha = true;
        public bool AutoAnimateClick = true;
        public bool AutoPlayButtonFx = false;

        public bool IsToggled { get; private set; }
        public bool IsLocked { get; private set; }
        public Button Bt { get { if (fooBt == null) fooBt = this.GetComponent<Button>(); return fooBt; } }
        public Image BtImg { get {
            if (fooBtImg == null) {
                fooBtImg = this.GetComponent<Image>();
                DefColor = fooBtImg.color;
            }
            return fooBtImg;
        }}
        public Image Ico { get { if (fooIco == null) fooIco = this.GetComponent<Image>(); return fooIco; } }
        public RectTransform RectT { get { if (fooRectT == null) fooRectT = this.GetComponent<RectTransform>(); return fooRectT; } }

        protected Color DefColor;
        Button fooBt;
        Image fooBtImg;
        bool fooIsToggled;
        Image fooIco;
        RectTransform fooRectT;
        Tween clickTween, pulseTween;

        #region Unity

        protected virtual void Awake()
        {
            clickTween = this.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.35f).SetAutoKill(false).SetUpdate(true).Pause();
            pulseTween = this.transform.DOScale(this.transform.localScale * 1.1f, 0.3f).SetAutoKill(false).SetUpdate(true).Pause()
                .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);

            Bt.onClick.AddListener(OnInternalClick);
        }

        protected virtual void OnDestroy()
        {
            clickTween.Kill();
            pulseTween.Kill();
            Bt.onClick.RemoveAllListeners();
        }

        #endregion

        #region Public Methods

        public void Toggle(bool _activate, bool _animateClick = false)
        {
            IsToggled = _activate;

            pulseTween.Rewind();
            BtImg.color = _activate ? DefColor : IsLocked ? BtLockedColor : BtToggleOffColor;
            if (ToggleIconAlpha && Ico != null) Ico.SetAlpha(_activate ? 1 : 0.4f);

            if (_animateClick) AnimateClick(true);
        }

        public virtual void Lock(bool _doLock)
        {
            IsLocked = _doLock;
            BtImg.color = _doLock ? BtLockedColor : IsToggled ? DefColor : BtToggleOffColor;
            Bt.interactable = !_doLock;
        }

        /// <summary>
        /// Pulsing stops automatically when the button is toggled or clicked (via <see cref="AnimateClick"/>)
        /// </summary>
        public void Pulse()
        {
            pulseTween.PlayForward();
        }

        public void StopPulsing()
        {
            pulseTween.Rewind();
        }

        public void AnimateClick(bool _force = false)
        {
            pulseTween.Rewind();
            if (AutoAnimateClick || _force) clickTween.Restart();
        }

        #endregion

        #region Callbacks

        void OnInternalClick()
        {
            AnimateClick();
            if (AutoPlayButtonFx) AudioManager.I.PlaySfx(Sfx.UIButtonClick);
        }

        #endregion
    }
}