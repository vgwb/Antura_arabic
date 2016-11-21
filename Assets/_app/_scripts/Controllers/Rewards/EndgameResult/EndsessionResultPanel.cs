// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/20

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EA4S
{
    public class EndsessionResultPanel : MonoBehaviour
    {
        [Header("Settings")]
        public float Godrays360Duration = 15f;
        [Header("References")]
        public EndsessionMinigames Minigames;
        public EndsessionBar Bar;
        public CanvasGroup GodraysCanvas;
        public RectTransform Godray0, Godray1;

        bool setupDone;
        Tween showTween, godraysTween;

        #region Unity + Setup

        void Setup()
        {
            if (setupDone) return;

            setupDone = true;

            showTween = DOTween.Sequence().SetAutoKill(false).Pause()
                .Append(this.GetComponent<Image>().DOFade(0, 0.35f).From().SetEase(Ease.Linear))
                .Join(GodraysCanvas.DOFade(0, 0.35f).From().SetEase(Ease.Linear))
                .OnRewind(() => {
                    this.gameObject.SetActive(false);
                    godraysTween.Pause();
                });
            godraysTween = DOTween.Sequence().SetAutoKill(false).Pause().SetLoops(-1, LoopType.Restart)
                .Append(Godray0.DORotate(new Vector3(0, 0, 360), Godrays360Duration, RotateMode.FastBeyond360).SetRelative().SetEase(Ease.Linear))
                .Join(Godray1.DORotate(new Vector3(0, 0, -360), Godrays360Duration, RotateMode.FastBeyond360).SetRelative().SetEase(Ease.Linear));

            this.gameObject.SetActive(false);
        }

        void Awake()
        {
            Setup();
        }

        void OnDestroy()
        {
            this.StopAllCoroutines();
            showTween.Kill();
            godraysTween.Kill();
        }

        #endregion

        #region Public Methods

        public void Show(List<EndsessionResultData> _sessionData, bool _immediate)
        {
            Setup();

            this.StopAllCoroutines();
            if (_immediate) showTween.Complete();
            else showTween.Restart();
            godraysTween.Restart();
            this.gameObject.SetActive(true);
            this.StartCoroutine(CO_Show(_sessionData));
        }

        public void Hide(bool _immediate)
        {
            if (!setupDone) return;

            this.StopAllCoroutines();
            if (_immediate) showTween.Rewind();
            else showTween.PlayBackwards();
            Bar.Hide();
            Minigames.Hide();
        }

        #endregion

        #region Methods

        IEnumerator CO_Show(List<EndsessionResultData> _sessionData)
        {
            yield return null;

            Bar.Hide();
            Minigames.Show(_sessionData);
            yield return new WaitForSeconds(1);

            Bar.Show();
        }

        #endregion
    }
}