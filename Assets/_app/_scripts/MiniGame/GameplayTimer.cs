using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ModularFramework.Core;
using TMPro;

namespace EA4S
{
    public class GameplayTimer : Singleton<GameplayTimer>
    {
        #region Settings
        [Range(10, 300)]
        public float time;
        private TextMeshProUGUI timerText;
        #endregion

        #region Runtime variables
        /// <summary>
        /// 
        /// </summary>
        protected int CurrentTime {
            get { return currentTime; }
            set {
                if (currentTime != (int)timeRemaining)
                    timeChanged((int)timeRemaining);
                currentTime = (int)timeRemaining;
            }
        }
        private int currentTime;
        private bool isRunning;
        private float timeRemaining;
        protected List<CustomEventData> CustomEvents;
        #endregion

        #region timer update
        void Update()
        {
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
        #endregion

        #region Timer functionalities
        /// <summary>
        /// Start timer with time count by param.
        /// </summary>
        /// <param name="_time"></param>
        public void StartTimer(float _time, List<CustomEventData> _customEvents = null)
        {
            if (OnStartTimer != null)
                OnStartTimer(_time);
            ResetTimer(_time);
            StartTimer();
            if (_customEvents != null)
                CustomEvents = _customEvents;
        }

        /// <summary>
        /// Start timer with default time count.
        /// </summary>
        public void StartTimer()
        {
            isRunning = true;
        }

        /// <summary>
        /// Stop timer.
        /// </summary>
        public void StopTimer()
        {
            isRunning = false;
        }

        /// <summary>
        /// Reset timer with default time count.
        /// </summary>
        public void ResetTimer()
        {
            timeRemaining = time;
        }

        /// <summary>
        /// Reset timer with time count by param.
        /// </summary>
        /// <param name="_time"></param>
        public void ResetTimer(float _time)
        {
            timeRemaining = _time;
        }

        public void DisplayTime()
        {
            if (!timerText)
                timerText = GetComponent<TextMeshProUGUI>();
            CurrentTime = (int)timeRemaining;
            var text = CurrentTime.ToString(); //Mathf.Floor(timeRemaining).ToString();
            timerText.text = text;
        }

        /// <summary>
        /// Force end time.
        /// </summary>
        public void EndTimeRemaning()
        {
            timeRemaining = 0;
        }
        #endregion

        #region events

        public delegate void TimerEvent(float _time);
        public delegate void CustomTimerEvent(CustomEventData _data);

        public static event TimerEvent OnStartTimer;
        public static event TimerEvent OnTimeOver;
        public static event CustomTimerEvent OnCustomEvent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_eventData"></param>
        public void RegisterCustomEvent(CustomEventData _eventData)
        {
            CustomEvents.Add(_eventData);
        }

        public struct CustomEventData
        {
            public string Name;
            public int Time;
        }

        /// <summary>
        /// Called every value change of int value of timer.
        /// </summary>
        /// <param name="_time"></param>
        void timeChanged(int _time)
        {
            foreach (var ev in CustomEvents.FindAll(e => e.Time == _time)) {
                if (OnCustomEvent != null)
                    OnCustomEvent(ev);
            }
        }
        #endregion
    }
}