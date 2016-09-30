using System;
using UnityEngine;

namespace EA4S.Tobogan
{
    public class ToboganGame : MiniGame
    {
        public PipesAnswerController pipesAnswerController;
        public GameObject questionLivingLetterPrefab;
        public QuestionLivingLettersBox questionLivingLetterBox;

        public QuestionsManager questionsManager;

        public int CurrentAnswersRecord = 0;
        public const int MAX_ANSWERS_RECORD = 15;

        const int STARS_1_THRESHOLD = 5;
        const int STARS_2_THRESHOLD = 8;
        const int STARS_3_THRESHOLD = 12;

        public int CurrentStars
        {
            get
            {
                if (CurrentAnswersRecord < STARS_1_THRESHOLD)
                    return 0;
                if (CurrentAnswersRecord < STARS_2_THRESHOLD)
                    return 1;
                if (CurrentAnswersRecord < STARS_3_THRESHOLD)
                    return 2;
                return 3;
            }
        }

        public ToboganIntroductionState IntroductionState { get; private set; }
        public ToboganPlayState PlayState { get; private set; }
        public ToboganResultGameState ResultState { get; private set; }

        protected override IGameConfiguration GetConfiguration()
        {
            return ToboganConfiguration.Instance;
        }

        protected override IGameState GetInitialState()
        {
            return IntroductionState;
        }

        protected override void OnInitialize(IGameContext context)
        {
            IntroductionState = new ToboganIntroductionState(this);
            PlayState = new ToboganPlayState(this);
            ResultState = new ToboganResultGameState(this);

            questionsManager = new QuestionsManager(this);
        }
    }
}