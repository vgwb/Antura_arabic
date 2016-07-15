using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ModularFramework.Core;
using TMPro;

namespace EA4S
{
    public class GameplayTimer : Singleton<GameplayTimer>
    {
        [Range(10, 300)]
        public float time;
        private TextMeshProUGUI timerText;

        private bool isRunning;
        private float timeRemaining;

        void Update() {
            if (!isRunning)
                return;
            if (timeRemaining > 0f) {
                timeRemaining -= Time.deltaTime;
            } else {
                if (OnTimeOver != null)
                    OnTimeOver(0);
                timeRemaining = 0;
                isRunning = false;
            }
            DisplayTime();
        }

        /// <summary>
        /// Start timer with time count by param.
        /// </summary>
        /// <param name="_time"></param>
        public void StartTimer(float _time) {
            if (OnStartTimer != null)
                OnStartTimer(_time);
            ResetTimer(_time);
            StartTimer();
        }

        /// <summary>
        /// Start timer with default time count.
        /// </summary>
        public void StartTimer() {
            isRunning = true;
        }

        /// <summary>
        /// Stop timer.
        /// </summary>
        public void StopTimer() {
            isRunning = false;
        }

        /// <summary>
        /// Reset timer with default time count.
        /// </summary>
        public void ResetTimer() {
            timeRemaining = time;
        }

        /// <summary>
        /// Reset timer with time count by param.
        /// </summary>
        /// <param name="_time"></param>
        public void ResetTimer(float _time) {
            timeRemaining = _time;
        }

        public void DisplayTime() {
            if (!timerText)
                timerText = GetComponent<TextMeshProUGUI>();
            var text = Mathf.Floor(timeRemaining).ToString();
            timerText.text = text;
        }

        #region events

        public delegate void TimerEvent(float _time);

        public static event TimerEvent OnStartTimer;
        public static event TimerEvent OnTimeOver;

        #endregion
    }
}