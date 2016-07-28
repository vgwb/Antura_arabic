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

        static SceneTransitioner I;
        const string ResourceId = "Prefabs/UI/SceneTransitioner";
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
            Init();

            IsShown = _doShow;
            if (_doShow) {
                I.MaskCover.fillClockwise = true;
                I.onRewindCallback = null;
                I.onCompleteCallback = _onComplete;
                I.tween.Restart();
            } else {
                I.MaskCover.fillClockwise = false;
                I.onCompleteCallback = null;
                I.onRewindCallback = _onComplete;
                if (I.tween.Elapsed() <= 0) {
                    I.tween.Pause();
                    I.OnRewind();
                } else I.tween.PlayBackwards();
            }
        }

        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■■ INTERNAL

        static void Init()
        {
            if (I != null) return;

            GameObject go = Instantiate(Resources.Load<GameObject>(ResourceId));
            go.name = "[SceneTransitioner]";
            DontDestroyOnLoad(go);
        }

        void Awake()
        {
            I = this.GetComponent<SceneTransitioner>();

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
            I = null;
            tween.Kill();
        }

        void OnRewind()
        {
            this.gameObject.SetActive(false);
            if (onRewindCallback != null) onRewindCallback();
        }

        void OnComplete()
        {
            if (onCompleteCallback != null) onCompleteCallback();
        }
    }
}