using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace EA4S.HideAndSeek
{
    public class HideAndSeekGame : MiniGame
    {
        public IntroductionGameState IntroductionState { get; private set; }
        public QuestionGameState QuestionState { get; private set; }
        public TutorialGameState TutorialState { get; private set; }
        public PlayGameState PlayState { get; private set; }
        public ResultGameState ResultState { get; private set; }
        
        public int CurrentScore { get; private set; }
        
        public HideAndSeekGameManager GameManager;

        public HideAndSeekTutorialManager TutorialManager;

        [HideInInspector]
        public bool isTimesUp;

        public bool inGame = false;

        const int STARS_1_THRESHOLD = 2;
        const int STARS_2_THRESHOLD = 5;
        const int STARS_3_THRESHOLD = 9;

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

        protected override void OnInitialize(IGameContext context)
        {
            IntroductionState = new IntroductionGameState(this);
            QuestionState = new QuestionGameState(this);
            TutorialState = new TutorialGameState(this);
            PlayState = new PlayGameState(this);
            ResultState = new ResultGameState(this);
        }

        public void ResetScore()
        {
            CurrentScore = 0;
        }

        protected override IGameState GetInitialState()
        {
            return IntroductionState;
        }

        protected override IGameConfiguration GetConfiguration()
        {
			return HideAndSeekConfiguration.Instance;
        }

        public void OnResult()
        {
            Context.GetOverlayWidget().SetStarsScore(++CurrentScore);
        }
    }
}
