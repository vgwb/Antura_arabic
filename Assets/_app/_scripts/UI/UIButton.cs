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
        public bool ToggleIconAlpha = true;

        public bool IsToggled { get; private set; }
        public Button Bt { get { if (fooBt == null) fooBt = this.GetComponent<Button>(); return fooBt; } }
        public RectTransform RectT { get { if (fooRectT == null) fooRectT = this.GetComponent<RectTransform>(); return fooRectT; } }

        Button fooBt;
        RectTransform fooRectT;
        Image btImg, ico;
        Color defColor;
        Tween clickTween, pulseTween;

        #region Unity

        protected virtual void Awake()
        {
            clickTween = this.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.35f).SetAutoKill(false).SetUpdate(true).Pause();
            pulseTween = this.transform.DOScale(this.transform.localScale * 1.1f, 0.3f).SetAutoKill(false).SetUpdate(true).Pause()
                .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        }

        protected virtual void OnDestroy()
        {
            clickTween.Kill();
            pulseTween.Kill();
        }

        #endregion

        #region Public Methods

        public void Toggle(bool _activate, bool _animateClick = false)
        {
            IsToggled = _activate;

            pulseTween.Rewind();
            if (btImg == null) {
                btImg = this.GetComponent<Image>();
                defColor = btImg.color;
            }
            if (ico == null) ico = this.GetOnlyComponentsInChildren<Image>(true)[0];

            btImg.color = _activate ? defColor : BtToggleOffColor;
            if (ToggleIconAlpha && ico != null) ico.SetAlpha(_activate ? 1 : 0.4f);

            if (_animateClick) AnimateClick();
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

        public void AnimateClick()
        {
            pulseTween.Rewind();
            clickTween.Restart();
        }

        #endregion
    }
}