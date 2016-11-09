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

        int lives = 3;

        public const int TIME_TO_ANSWER = 20;
        const int QUESTIONS = 5;
        const int STARS_1_THRESHOLD = 10;
        const int STARS_2_THRESHOLD = 20;
        const int STARS_3_THRESHOLD = 30;

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
        public IQuestionPack CurrentQuestion { get; set; }

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

            Context.GetOverlayWidget().Initialize(true, true, true);
            Context.GetOverlayWidget().SetMaxLives(lives);
            Context.GetOverlayWidget().SetLives(lives);
            Context.GetOverlayWidget().SetStarsThresholds(STARS_1_THRESHOLD, STARS_2_THRESHOLD, STARS_3_THRESHOLD);
        }

        public void AddScore(int score)
        {
            CurrentScore += score;

            Context.GetOverlayWidget().SetStarsScore(CurrentScore);
        }

        public bool RemoveLife()
        {
            --lives;
            Context.GetOverlayWidget().SetLives(lives);

            if (lives == 0)
            {
                EndGame(0, 0);
                return true;
            }
            return false;
        }
    }
}