using System;
using TMPro;
using UnityEngine;

namespace EA4S.ReadingGame
{
    public class ReadingGameGame : MiniGame // ReadingGameGameGameGameGame!
    {
        public ReadingBarSet barSet;
        public GameObject blurredText;
        public GameObject circleBox;

        public int CurrentScore { get; private set; }

        [HideInInspector]
        public bool isTimesUp;

        const int STARS_1_THRESHOLD = 3;
        const int STARS_2_THRESHOLD = 5;
        const int STARS_3_THRESHOLD = 6;

        public int CurrentStars
        {
            get
            {
                if (CurrentScore < STARS_1_THRESHOLD)
                    return 0;
                if (CurrentScore < STARS_2_THRESHOLD)
                    return 1;
                if (CurrentScore < STARS_3_THRESHOLD)
                    return 2;
                return 3;
            }
        }

        public ReadingGameReadState ReadState { get; private set; }
        public ReadingGameAnswerState AnswerState { get; private set; }

        protected override IGameConfiguration GetConfiguration()
        {
            return ReadingGameConfiguration.Instance;
        }

        protected override IGameState GetInitialState()
        {
            return ReadState;
        }

        protected override void OnInitialize(IGameContext context)
        {
            ReadState = new ReadingGameReadState(this);
            AnswerState = new ReadingGameAnswerState(this);

            Context.GetOverlayWidget().Initialize(true, true, false);
            Context.GetOverlayWidget().SetStarsThresholds(STARS_1_THRESHOLD, STARS_2_THRESHOLD, STARS_3_THRESHOLD);
        }

        void OnResult(bool result)
        {
            Context.GetCheckmarkWidget().Show(result);

            if (result)
            {
                ++CurrentScore;
            }
            else
            {
                CurrentScore = 0;
            }

            Context.GetOverlayWidget().SetStarsScore(CurrentScore);
        }
    }
}