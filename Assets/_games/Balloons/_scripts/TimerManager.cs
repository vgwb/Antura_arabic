using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EA4S;
using TMPro;

namespace EA4S.Balloons
{
    public class TimerManager : MonoBehaviour
    {
        [HideInInspector]
        public float time;
        //public TextMeshProUGUI timerText;

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
                if (!playedSfx && timeRemaining < 4.5f)
                {
                    BalloonsConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.DangerClockLong);
                    playedSfx = true;
                }
                if (timeRemaining <= 0f)
                {
                    StopTimer();
                    BalloonsGame.instance.OnTimeUp();
                }
            }
        }

        public void InitTimer()
        {
            time = BalloonsGame.instance.roundTime;
            if (MinigamesUI.Timer != null)
            {
                MinigamesUI.Timer.Setup(time);
            }
        }

        public void StartTimer()
        {
            isRunning = true;
            MinigamesUI.Timer.Play();
        }

        public void StopTimer()
        {
            isRunning = false;
            playedSfx = false;
            if (MinigamesUI.Timer != null)
            {
                MinigamesUI.Timer.Pause();
            }
            //AudioManager.I.StopSfx(Sfx.DangerClockLong);
        }

        public void ResetTimer()
        {
            if (MinigamesUI.Timer == null)
            {
                return;
            }
            if (!MinigamesUI.Timer.IsSetup)
            {
                InitTimer();
            }
            StopTimer();
            timeRemaining = time;
            MinigamesUI.Timer.Rewind();
        }

        public void DisplayTime()
        {
            //textvar text = Mathf.Floor(timeRemaining).ToString();
            //timerText.text = text;
        }
    }
}