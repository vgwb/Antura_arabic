using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EA4S;

namespace Balloons
{
    public class TimerManager : MonoBehaviour
    {
        [Range(10, 300)]
        public float time;
        public Text timerText;

        private bool isRunning;
        private bool playedSfx;
        private float timeRemaining;

        void Update()
        {
            if (isRunning)
            {
                if (timeRemaining > 0f)
                {
                    timeRemaining -= Time.deltaTime;
                    DisplayTime();
                }
                if (!playedSfx && timeRemaining < 5f)
                {
                    AudioManager.I.PlaySfx(Sfx.DangerClock);
                    playedSfx = true;
                }
                if (timeRemaining < 1f)
                {
                    StopTimer();
                    BalloonsGameManager.instance.OnTimeUp();
                }
            }

        }

        public void StartTimer()
        {
            isRunning = true;
        }

        public void StopTimer()
        {
            isRunning = false;
            playedSfx = false;
            AudioManager.I.StopSfx(Sfx.DangerClock);
        }

        public void ResetTimer()
        {
            timeRemaining = time;
        }

        public void DisplayTime()
        {
            var text = Mathf.Floor(timeRemaining).ToString();
            timerText.text = text;
        }
    }
}