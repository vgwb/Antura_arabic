using UnityEngine;

namespace EA4S
{
    public interface IStarsBarWidget
    {
        void Show(int firstStarsScoreThreshold, int secondStarsScoreThreshold, int thirdStarsScoreThreshold);
        void SetScore(int score);

        void Hide();
    }
}
