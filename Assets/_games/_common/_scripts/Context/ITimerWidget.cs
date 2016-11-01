using UnityEngine;

namespace EA4S
{
    public interface ITimerWidget
    {
        void Show();
        void SetDuration(float timerDuration);
        void SetTime(float currentTime);

        void Hide();
    }
}
