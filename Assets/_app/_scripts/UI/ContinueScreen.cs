using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EA4S
{
    public enum ContinueScreenMode
    {
        /// <summary>No button, just touch the screen to continue</summary>
        Fullscreen,
        /// <summary>Button that needs to be clicked directly</summary>
        Button,
        /// <summary>Button, but the whole screen can be clicked</summary>
        ButtonFullscreen
    }

    public class ContinueScreen : MonoBehaviour
    {
        public Button Bg, BtContinue;

        public static bool IsShown { get; private set; }

        static ContinueScreen I;
        const string ResourceId = "Prefabs/UI/ContinueScreen";
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
        public static void Show(Action _onContinue, ContinueScreenMode _mode = ContinueScreenMode.ButtonFullscreen)
        {
            Init();

            Debug.Log(_mode);
            IsShown = true;
            I.clicked = false;
            I.currMode = _mode;
            I.onContinueCallback = _onContinue;
            I.BtContinue.gameObject.SetActive(_mode != ContinueScreenMode.Fullscreen);
            I.gameObject.SetActive(true);
            I.showTween.Restart();
        }

        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■■ INTERNAL

        static void Init()
        {
            if (I != null) return;

            GameObject go = Instantiate(Resources.Load<GameObject>(ResourceId));
            go.name = "[ContinueScreen]";
            DontDestroyOnLoad(go);
        }

        void Awake()
        {
            I = this.GetComponent<ContinueScreen>();

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
            I = null;
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

            clicked = true;
            if (_isButton || currMode == ContinueScreenMode.ButtonFullscreen) btTween.Restart();
            else if (currMode == ContinueScreenMode.Fullscreen) Continue();
        }
    }
}