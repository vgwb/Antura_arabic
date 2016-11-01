using System;

namespace EA4S
{
    public class MinigamesTimerWidget : ITimerWidget
    {
        float timerDuration = 10;

        public void Show()
        {
            MinigameUIProxy.I.Timer.gameObject.SetActive(true);
        }

        public void Hide()
        {
            MinigameUIProxy.I.Timer.gameObject.SetActive(false);
        }

        public void SetDuration(float timerDuration)
        {
            this.timerDuration = timerDuration;
            MinigameUIProxy.I.Timer.Setup(timerDuration, false);
        }

        public void SetTime(float currentTime)
        {
            MinigameUIProxy.I.Timer.Goto(timerDuration - currentTime);
        }
    }
}