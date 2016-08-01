using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using DG.DeExtensions;

namespace EA4S
{
    public enum ContinueScreenMode {
        /// <summary>Just background, just touch the screen to continue</summary>
        FullscreenBg,
        /// <summary>Button with no background, that needs to be clicked directly</summary>
        Button,
        /// <summary>Button with no background, the whole screen can be clicked (button will be placed on the side)</summary>
        ButtonFullscreen,
        /// <summary>Button with background, that needs to be clicked directly</summary>
        ButtonWithBg,
        /// <summary>Button with background, the whole screen can be clicked</summary>
        ButtonWithBgFullscreen
    }

    public class ContinueScreen : MonoBehaviour
    {
        [Header("Settings")]
        public Vector2 BtSidePosition;
        [Header("References")]
        public Button Bg;
        public Button BtContinue;

        public static bool IsShown { get; private set; }

        Vector2 btCenteredPosition;
        RectTransform btRT;
        ContinueScreenMode currMode;
        Action onContinueCallback;
        bool clicked;
        Tween showTween, showBgTween, btClickTween, btIdleTween;

        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■■ PUBLIC METHODS

        /// <summary>
        /// Call this to show/hide the continue screen.
        /// </summary>
        /// <param name="_onContinue">Eventual callback to call when the user clicks to continue</param>
        /// <param name="_mode">Mode</param>
        public static void Show(Action _onContinue, ContinueScreenMode _mode = ContinueScreenMode.ButtonWithBg) {
            GlobalUI.Init();
            GlobalUI.ContinueScreen.DoShow(_onContinue, _mode);
        }

        void DoShow(Action _onContinue, ContinueScreenMode _mode = ContinueScreenMode.ButtonWithBg) {
            IsShown = true;
            clicked = false;
            currMode = _mode;
            onContinueCallback = _onContinue;
            Bg.gameObject.SetActive(_mode != ContinueScreenMode.Button);
            BtContinue.gameObject.SetActive(_mode != ContinueScreenMode.FullscreenBg);
            if (_mode == ContinueScreenMode.ButtonFullscreen) {
                btRT.anchorMax = btRT.anchorMin = Vector2.zero;
                btRT.anchoredPosition = BtSidePosition;
            } else {
                btRT.anchorMax = btRT.anchorMin = new Vector2(0.5f, 0.5f);
                btRT.anchoredPosition = btCenteredPosition;
            }
            showBgTween.Rewind();
            showTween.Restart();
            if (_mode != ContinueScreenMode.Button && _mode != ContinueScreenMode.ButtonFullscreen) showBgTween.PlayForward();
            this.gameObject.SetActive(true);
        }

        /// <summary>
        /// Closes the continue screen without dispatching any callback
        /// </summary>
        /// <param name="_immediate">If TRUE, immmediately closes the screen without animation</param>
        public static void Close(bool _immediate) {
            GlobalUI.ContinueScreen.DoClose(_immediate);
        }

        void DoClose(bool _immediate) {
            if (!IsShown && !_immediate)
                return;

            IsShown = false;
            clicked = false;
            onContinueCallback = null;
            if (_immediate) {
                showTween.Rewind();
                showBgTween.Rewind();
                this.gameObject.SetActive(false);
            } else
                showTween.PlayBackwards();
                showBgTween.PlayBackwards();
        }

        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■■ INTERNAL

        void Awake() {
            btRT = BtContinue.GetComponent<RectTransform>();
            btCenteredPosition = btRT.anchoredPosition;

            const float duration = 0.5f;
            showTween = btRT.DOScale(0.1f, duration).From().SetEase(Ease.OutBack)
                .SetUpdate(true).SetAutoKill(false).Pause()
                .OnPlay(() => this.gameObject.SetActive(true))
                .OnRewind(() => {
                    this.gameObject.SetActive(false);
                    btIdleTween.Rewind();
                })
                .OnComplete(() => {
                    if (currMode == ContinueScreenMode.ButtonFullscreen) btIdleTween.Restart();
                });

            showBgTween = Bg.image.DOFade(0, duration).From().SetEase(Ease.InSine)
                .SetUpdate(true).SetAutoKill(false).Pause();

            btClickTween = btRT.DOPunchRotation(new Vector3(0, 0, 20), 0.3f, 12, 0.5f).SetUpdate(true).SetAutoKill(false).Pause()
                .OnComplete(Continue);

            btIdleTween = btRT.DOAnchorPosX(10, 0.5f).SetRelative().SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true).SetAutoKill(false).Pause();

            this.gameObject.SetActive(false);

            // Listeners
            BtContinue.onClick.AddListener(() => OnClick(true));
            Bg.onClick.AddListener(() => OnClick(false));
        }

        void OnDestroy() {
            showTween.Kill();
            showBgTween.Kill();
            btClickTween.Kill();
            btIdleTween.Kill();
            BtContinue.onClick.RemoveAllListeners();
            Bg.onClick.RemoveAllListeners();
        }

        void Continue() {
            if (onContinueCallback != null)
                onContinueCallback();
            showTween.PlayBackwards();
            showBgTween.PlayBackwards();
        }

        void OnClick(bool _isButton) {
            if (clicked)
                return;

            if (_isButton || currMode == ContinueScreenMode.ButtonWithBgFullscreen || currMode == ContinueScreenMode.ButtonFullscreen) {
                clicked = true;
                btClickTween.Restart();
            } else if (currMode == ContinueScreenMode.FullscreenBg) {
                clicked = true;
                Continue();
            }
            if (clicked) {
                btIdleTween.Pause();
                AudioManager.I.PlaySfx(Sfx.UIButtonClick);
            }
        }
    }
}