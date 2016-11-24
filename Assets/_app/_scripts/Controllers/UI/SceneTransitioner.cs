// Author: Daniele Giardini - http://www.demigiant.com

using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EA4S
{
    public class SceneTransitioner : MonoBehaviour
    {
        [Header("Options")]
        public float AnimationDuration = 0.75f;
        [Header("References")]
        public Image MaskCover;
        public RectTransform Icon, Logo;

        public static bool IsShown { get; private set; }
        public static bool IsPlaying { get; private set; }

        Action onCompleteCallback, onRewindCallback;
        Sequence tween;

        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■■ PUBLIC METHODS

        /// <summary>
        /// Call this to show/hide the scene transitioner.
        /// </summary>
        /// <param name="_doShow">If TRUE animates the transition IN and stops when the screen is covered, otherwise animates OUT</param>
        /// <param name="_onComplete">Eventual callback to call when the transition IN/OUT completes</param>
        public static void Show(bool _doShow, Action _onComplete = null)
        {
            GlobalUI.Init();

            GlobalUI.SceneTransitioner.DoShow(_doShow, _onComplete);
        }

        public static void Close()
        {
            if (IsShown) {
                Show(false);
            }
        }

        void DoShow(bool _doShow, Action _onComplete = null)
        {
            IsShown = _doShow;
            if (_doShow) {
                MaskCover.fillClockwise = true;
                onRewindCallback = null;
                onCompleteCallback = _onComplete;
                tween.Restart();
                this.gameObject.SetActive(true);
                AudioManager.I.PlaySfx(Sfx.Transition);
                IsPlaying = true;
            } else {
                MaskCover.fillClockwise = false;
                onCompleteCallback = null;
                onRewindCallback = _onComplete;
                if (tween.Elapsed() <= 0) {
                    tween.Pause();
                    OnRewind();
                } else
                    tween.PlayBackwards();
            }
        }

        public void CloseImmediate()
        {
            onRewindCallback = null;
            tween.Rewind();
        }

        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■■ INTERNAL

        void Awake()
        {
            tween = DOTween.Sequence().SetUpdate(true).SetAutoKill(false).Pause()
                .Append(MaskCover.DOFillAmount(0, AnimationDuration).From())
                .Join(Icon.DOScale(0.01f, AnimationDuration * 0.6f).From())
                .Join(Icon.DOPunchRotation(new Vector3(0, 0, 90), AnimationDuration * 0.9f, 6))
                .Insert(AnimationDuration * 0.4f, Logo.DOScale(0.01f, AnimationDuration * 0.5f).From().SetEase(Ease.OutBack))
                .OnPlay(() => this.gameObject.SetActive(true))
                .OnRewind(OnRewind)
                .OnComplete(OnComplete);

            this.gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            tween.Kill();
        }

        void OnRewind()
        {
            IsPlaying = false;
            this.gameObject.SetActive(false);
            if (onRewindCallback != null)
                onRewindCallback();
        }

        void OnComplete()
        {
            GlobalUI.Clear(false);
            if (onCompleteCallback != null)
                onCompleteCallback();
        }
    }
}