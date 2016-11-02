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

        public float Duration { get; private set; }
        public float Elapsed { get; private set; }
        Sequence timerTween, shakeTween;
        Tween endTween;

        #region Unity

        void Awake()
        {
            if (!IsSetup) {
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
        /// Sets the duration of the timer. Call this before calling any other method.
        /// </summary>
        /// <param name="_timerDuration">Timer duration in seconds</param>
        /// <param name="_playImmediately">If TRUE the timer starts immediately, otherwise waits for a <see cref="Play"/> call</param>
        public void Setup(float _timerDuration, bool _playImmediately = false)
        {
            Duration = _timerDuration;
            Elapsed = 0;

            if (IsSetup) {
                timerTween.Rewind();
                timerTween.Kill();
                shakeTween.Kill(true);
                endTween.Kill(true);
            }

            TfTimer.text = Duration.ToString();

            // Shake tween
            float duration = Duration * 0.25f;
            shakeTween = DOTween.Sequence().SetAutoKill(false)
                .Append(this.transform.DOShakeRotation(duration, new Vector3(0, 0, 20f), 20))
                .Join(this.transform.DOShakeScale(duration, 0.1f, 20));
            shakeTween.Complete();

            // End tween
            endTween = this.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f).SetAutoKill(false).Pause();

            // Timer tween
            Radial.fillAmount = 0;
            timerTween = DOTween.Sequence().SetAutoKill(false)
                .Append(Radial.DOFillAmount(1, Duration).SetEase(Ease.Linear))
                .Join(Radial.DOColor(EndColor, Duration).SetEase(Ease.Linear))
                .OnUpdate(() => {
                    Elapsed = timerTween.Elapsed();
                    TfTimer.text = Mathf.CeilToInt(Duration - Elapsed).ToString();
                    float elapsedPercentage = timerTween.ElapsedPercentage();
                    float shakePercentage = elapsedPercentage < 0.75f ? 0 : (1 - elapsedPercentage) / 0.25f;
                    shakeTween.Goto(shakeTween.Duration() * shakePercentage);
                })
                .OnComplete(() => {
                    shakeTween.Rewind();
                    endTween.Restart();
                })
                .OnPause(() => {
                    if (!timerTween.IsComplete() && shakeTween.IsPlaying()) shakeTween.Pause();
                });
            if (!_playImmediately) timerTween.Pause();

            IsSetup = true;
        }

        /// <summary>Plays the timer</summary>
        public void Play()
        {
            if (!Validate("MinigamesUITimer")) return;

            timerTween.Play();
        }

        /// <summary>Pauses the timer</summary>
        public void Pause()
        {
            if (!Validate("MinigamesUITimer")) return;

            timerTween.Pause();
        }

        /// <summary>Rewinds then restarts the timer</summary>
        public void Restart()
        {
            if (!Validate("MinigamesUITimer")) return;

            endTween.Rewind();
            timerTween.Restart();
        }

        /// <summary>Rewinds the timer and pauses it</summary>
        public void Rewind()
        {
            if (!Validate("MinigamesUITimer")) return;

            shakeTween.Complete();
            endTween.Rewind();
            timerTween.Rewind();
        }

        /// <summary>Completes the timer</summary>
        public void Complete()
        {
            if (!Validate("MinigamesUITimer")) return;

            timerTween.Complete();
        }

        /// <summary>Sends the timer to the given time (in seconds)</summary>
        /// <param name="_time">Time (in seconds) to go to</param>
        /// <param name="_andPlay">If TRUE also plays the timer after going to the given position, otherwise pauses it</param>
        public void Goto(float _time, bool _andPlay = false)
        {
            if (!Validate("MinigamesUITimer")) return;
            if (_time > timerTween.Duration()) _time = timerTween.Duration();
            if (Mathf.Approximately(_time, timerTween.Elapsed())) return;

            endTween.Rewind();
            timerTween.Goto(_time, _andPlay);
        }

        /// <summary>Sends the timer to the given time percentage (<code>0 to 1</code>)</summary>
        /// <param name="_percentage">Time percentage (<code>0 to 1</code>) to go to</param>
        /// <param name="_andPlay">If TRUE also plays the timer after going to the given position, otherwise pauses it</param>
        public void GotoPercentage(float _percentage, bool _andPlay = false)
        {
            if (!Validate("MinigamesUITimer")) return;
            if (_percentage > 1) _percentage = 1;
            if (Mathf.Approximately(_percentage, timerTween.ElapsedPercentage())) return;

            endTween.Rewind();
            timerTween.Goto(timerTween.Duration() * _percentage, _andPlay);
        }

        #endregion
    }
}