using UnityEngine;

namespace EA4S
{
    public interface IOverlayWidget
    {
        void Initialize(bool showStarsBar, bool showClock, bool showLives);

        void SetStarsThresholds(int firstStarsScoreThreshold, int secondStarsScoreThreshold, int thirdStarsScoreThreshold);
        void SetStarsScore(int score);
        
        void SetClockDuration(float timerDuration);
        void SetClockTime(float currentTime);
        void OnClockCompleted();

        void SetMaxLives(int maxLives);
        void SetLives(int lives);

        void Reset();
    }
}
