using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EA4S
{
    public enum ContinueScreenMode
    {
        /// <summary>Just background, just touch the screen to continue</summary>
        FullscreenBg,
        /// <summary>Button with no background, that needs to be clicked directly</summary>
        Button,
        /// <summary>Button with background, that needs to be clicked directly</summary>
        ButtonWithBg,
        /// <summary>Button with background, but the whole screen can be clicked</summary>
        ButtonWithBgFullscreen
    }

    public class ContinueScreen : MonoBehaviour
    {
        public Button Bg, BtContinue;

        public static bool IsShown { get; private set; }

        ContinueScreenMode currMode;
        Action onContinueCallback;
        bool clicked;
        Tween showTween, btTween;

        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■■ PUBLIC METHODS

        /// <summary>
        /// Call this to show/hide the continue screen.
        /// </summary>
        /// <param name="_onContinue">Eventual callback to call when the user clicks to continue</param>
        /// <param name="_mode">Mode</param>
        public static void Show(Action _onContinue, ContinueScreenMode _mode = ContinueScreenMode.ButtonWithBg)
        {
            GlobalUI.Init();
            GlobalUI.ContinueScreen.DoShow(_onContinue, _mode);
        }
        void DoShow(Action _onContinue, ContinueScreenMode _mode = ContinueScreenMode.ButtonWithBg)
        {
            IsShown = true;
            clicked = false;
            currMode = _mode;
            onContinueCallback = _onContinue;
            Bg.gameObject.SetActive(_mode != ContinueScreenMode.Button);
            BtContinue.gameObject.SetActive(_mode != ContinueScreenMode.FullscreenBg);
            showTween.Restart();
            this.gameObject.SetActive(true);
        }

        /// <summary>
        /// Closes the continue screen without dispatching any callback
        /// </summary>
        /// <param name="_immediate">If TRUE, immmediately closes the screen without animation</param>
        public static void Close(bool _immediate)
        {
            GlobalUI.ContinueScreen.DoClose(_immediate);
        }

        void DoClose(bool _immediate)
        {
            if (!IsShown) return;

            IsShown = false;
            clicked = false;
            onContinueCallback = null;
            if (_immediate) {
                showTween.Rewind();
                this.gameObject.SetActive(false);
            } else showTween.PlayBackwards();
        }

        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■■ INTERNAL

        void Awake()
        {
            RectTransform btRT = BtContinue.GetComponent<RectTransform>();

            const float duration = 0.5f;
            showTween = DOTween.Sequence().SetUpdate(true).SetAutoKill(false).Pause()
                .Append(Bg.GetComponent<Image>().DOFade(0, duration).From().SetEase(Ease.InSine))
                .Join(btRT.DOScale(0.1f, duration).From().SetEase(Ease.OutBack))
                .OnPlay(() => this.gameObject.SetActive(true))
                .OnRewind(()=> this.gameObject.SetActive(false));

            btTween = btRT.DOPunchRotation(new Vector3(0, 0, 20), 0.3f, 12, 0.5f).SetUpdate(true).SetAutoKill(false).Pause()
                .OnComplete(Continue);

            this.gameObject.SetActive(false);

            // Listeners
            BtContinue.onClick.AddListener(()=> OnClick(true));
            Bg.onClick.AddListener(()=> OnClick(false));
        }

        void OnDestroy()
        {
            showTween.Kill();
            BtContinue.onClick.RemoveAllListeners();
            Bg.onClick.RemoveAllListeners();
        }

        void Continue()
        {
            if (onContinueCallback != null) onContinueCallback();
            showTween.PlayBackwards();
        }

        void OnClick(bool _isButton)
        {
            if (clicked) return;

            if (_isButton || currMode == ContinueScreenMode.ButtonWithBgFullscreen) {
                clicked = true;
                btTween.Restart();
            } else if (currMode == ContinueScreenMode.FullscreenBg) {
                clicked = true;
                Continue();
            }
        }
    }
}