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
        public RectTransform Icon;

        public static bool IsShown { get; private set; }

        static SceneTransitioner instance;
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
                instance.MaskCover.fillClockwise = true;
                instance.onRewindCallback = null;
                instance.onCompleteCallback = _onComplete;
                instance.tween.Restart();
            } else {
                instance.MaskCover.fillClockwise = false;
                instance.onCompleteCallback = null;
                instance.onRewindCallback = _onComplete;
                if (instance.tween.Elapsed() <= 0) {
                    instance.tween.Pause();
                    instance.OnRewind();
                } else instance.tween.PlayBackwards();
            }
        }

        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■■ INTERNAL

        static void Init()
        {
            if (instance != null) return;

            GameObject go = Instantiate(Resources.Load<GameObject>(ResourceId));
            go.name = "[SceneTransitioner]";
            DontDestroyOnLoad(go);
            instance = go.GetComponent<SceneTransitioner>();
        }

        void Awake()
        {
            tween = DOTween.Sequence().SetUpdate(true).SetAutoKill(false).Pause()
                .Append(MaskCover.DOFillAmount(0, AnimationDuration).From())
                .Join(Icon.DOScale(0.01f, AnimationDuration * 0.3f).From())
                .Join(Icon.DOPunchRotation(new Vector3(0, 0, 60), AnimationDuration * 0.9f, 6))
                .OnPlay(() => this.gameObject.SetActive(true))
                .OnRewind(OnRewind)
                .OnComplete(OnComplete);

            this.gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            instance = null;
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