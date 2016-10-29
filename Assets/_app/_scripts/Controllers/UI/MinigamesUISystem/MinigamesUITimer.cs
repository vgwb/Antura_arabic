// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/10/28

using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EA4S
{
    public class MinigamesUITimer : ABSMinigamesUIComponent
    {
        public Image Radial;
        public TextMeshProUGUI TfTimer;
        public Color EndColor = Color.red;

        public bool IsSetup { get; private set; }
        float time;
        Sequence timerTween, shakeTween;
        Tween endTween;

        #region Unity

        void Awake()
        {
            if (timerTween == null) {
                Radial.fillAmount = 0;
                TfTimer.text = "";
            }
        }

        void OnDestroy()
        {
            timerTween.Kill();
            shakeTween.Kill();
            endTween.Kill();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the duration of the timer. Call this before calling any other Timer method.
        /// </summary>
        /// <param name="_timerDuration">Timer duration in seconds</param>
        /// <param name="_playImmediately">If TRUE the timer starts immediately, otherwise waits for a <see cref="Play"/> call</param>
        public void Setup(float _timerDuration, bool _playImmediately = false)
        {
            IsSetup = true;
            time = _timerDuration;

            if (timerTween != null) {
                timerTween.Rewind();
                timerTween.Kill();
                shakeTween.Kill(true);
                endTween.Kill(true);
            }

            TfTimer.text = time.ToString();

            // Shake tween
            float duration = time * 0.25f;
            shakeTween = DOTween.Sequence().SetAutoKill(false)
                .Append(this.transform.DOShakeRotation(duration, new Vector3(0, 0, 20f), 20))
                .Join(this.transform.DOShakeScale(duration, 0.1f, 20));
            shakeTween.Complete();

            // End tween
            endTween = this.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f).SetAutoKill(false).Pause();

            // Timer tween
            Radial.fillAmount = 0;
            timerTween = DOTween.Sequence().SetAutoKill(false)
                .Append(Radial.DOFillAmount(1, time).SetEase(Ease.Linear))
                .Join(Radial.DOColor(EndColor, time).SetEase(Ease.Linear))
                .OnUpdate(() => {
                    TfTimer.text = Mathf.CeilToInt(time - timerTween.Elapsed()).ToString();
                    if (timerTween.ElapsedPercentage() >= 0.75f) shakeTween.PlayBackwards();
                    else shakeTween.Complete();
                })
                .OnComplete(() => {
                    shakeTween.Rewind();
                    endTween.Restart();
                })
                .OnPause(() => {
                    if (!timerTween.IsComplete() && shakeTween.IsPlaying()) shakeTween.Pause();
                });

            if (!_playImmediately) timerTween.Pause();
        }

        /// <summary>Plays the timer</summary>
        public void Play()
        {
            if (timerTween == null) {
                Debug.LogWarning("MinigamesUITimer ► Timer duration not set: call Timer.Setup first");
                return;
            }

            timerTween.Play();
        }

        /// <summary>Pauses the timer</summary>
        public void Pause()
        {
            if (timerTween == null) {
                Debug.LogWarning("MinigamesUITimer ► Timer duration not set: call Timer.Setup first");
                return;
            }

            timerTween.Pause();
        }

        /// <summary>Rewinds then restarts the timer</summary>
        public void Restart()
        {
            if (timerTween == null) {
                Debug.LogWarning("MinigamesUITimer ► Timer duration not set: call Timer.Setup first");
                return;
            }

            endTween.Rewind();
            timerTween.Restart();
        }

        /// <summary>Rewinds the timer and pauses it</summary>
        public void Rewind()
        {
            if (timerTween == null) {
                Debug.LogWarning("MinigamesUITimer ► Timer duration not set: call Timer.Setup first");
                return;
            }

            shakeTween.Complete();
            endTween.Rewind();
            timerTween.Rewind();
        }

        /// <summary>Completes the timer</summary>
        public void Complete()
        {
            if (timerTween == null) {
                Debug.LogWarning("MinigamesUITimer ► Timer duration not set: call Timer.Setup first");
                return;
            }

            timerTween.Complete();
        }

        /// <summary>Sends the timer to the given time (in seconds)</summary>
        /// <param name="_time">Time (in seconds) to go to</param>
        /// <param name="_andPlay">If TRUE also plays the timer after going to the given position, otherwise pauses it</param>
        public void Goto(float _time, bool _andPlay = false)
        {
            if (timerTween == null) {
                Debug.LogWarning("MinigamesUITimer ► Timer duration not set: call Timer.Setup first");
                return;
            }

            endTween.Rewind();
            timerTween.Goto(_time, _andPlay);
        }

        /// <summary>Sends the timer to the given time percentage (<code>0 to 1</code>)</summary>
        /// <param name="_percentage">Time percentage (<code>0 to 1</code>) to go to</param>
        /// <param name="_andPlay">If TRUE also plays the timer after going to the given position, otherwise pauses it</param>
        public void GotoPercentage(float _percentage, bool _andPlay = false)
        {
            if (timerTween == null) {
                Debug.LogWarning("MinigamesUITimer ► Timer duration not set: call Timer.Setup first");
                return;
            }

            endTween.Rewind();
            timerTween.Goto(timerTween.Duration() * _percentage, _andPlay);
        }

        #endregion
    }
}