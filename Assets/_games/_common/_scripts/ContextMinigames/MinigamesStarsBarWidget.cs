using System;
using UnityEngine;

namespace EA4S
{
    public class MinigamesStarsBarWidget : IStarsBarWidget
    {
        int firstStarsScoreThreshold;
        int secondStarsScoreThreshold;
        int thirdStarsScoreThreshold;
        int score = 0;

        public void Show(int firstStarsScoreThreshold, int secondStarsScoreThreshold, int thirdStarsScoreThreshold)
        {
            this.firstStarsScoreThreshold = firstStarsScoreThreshold;
            this.secondStarsScoreThreshold = secondStarsScoreThreshold;
            this.thirdStarsScoreThreshold = thirdStarsScoreThreshold;
            MinigameUIProxy.I.Starbar.gameObject.SetActive(true);
        }

        public void Hide()
        {
            MinigameUIProxy.I.Starbar.gameObject.SetActive(false);
        }

        public void SetScore(int score)
        {
            this.score = score;

            // Avoid floating point errors when setting UI
            // thresholds could have different "distances" between them

            if (score < firstStarsScoreThreshold)
                MinigameUIProxy.I.Starbar.Goto((score / (float)firstStarsScoreThreshold) * 0.333f);
            else if (score == firstStarsScoreThreshold)
                MinigameUIProxy.I.Starbar.GotoStar(0);
            else if(score < secondStarsScoreThreshold)
                MinigameUIProxy.I.Starbar.Goto(((score - firstStarsScoreThreshold) / (float)(secondStarsScoreThreshold - firstStarsScoreThreshold)) * 0.333f + 0.333f);
            else if (score == secondStarsScoreThreshold)
                MinigameUIProxy.I.Starbar.GotoStar(1);
            else if(score < thirdStarsScoreThreshold)
                MinigameUIProxy.I.Starbar.Goto(((score - secondStarsScoreThreshold) / (float)(thirdStarsScoreThreshold - secondStarsScoreThreshold)) * 0.333f + 0.666f);
            else
                MinigameUIProxy.I.Starbar.GotoStar(2);
            
        }
    }
}
