using System;
using TMPro;
using UnityEngine;

namespace EA4S.Tobogan
{
    public class ToboganGame : MiniGame
    {
        public PipesAnswerController pipesAnswerController;
        public GameObject questionLivingLetterPrefab;
        public QuestionLivingLettersBox questionLivingLetterBox;
		public Camera tubesCamera;
        public ToboganFeedbackGraphics feedbackGraphics;

        public QuestionsManager questionsManager;

        public int CurrentScore { get; private set; }
        public int CurrentScoreRecord { get; private set; }

        [HideInInspector]
        public bool isTimesUp;

        public const int MAX_ANSWERS_RECORD = 15;

        const int STARS_1_THRESHOLD = 5;
        const int STARS_2_THRESHOLD = 8;
        const int STARS_3_THRESHOLD = 12;

        public int CurrentStars
        {
            get
            {
                if (CurrentScoreRecord < STARS_1_THRESHOLD)
                    return 0;
                if (CurrentScoreRecord < STARS_2_THRESHOLD)
                    return 1;
                if (CurrentScoreRecord < STARS_3_THRESHOLD)
                    return 2;
                return 3;
            }
        }

        public ToboganIntroductionState IntroductionState { get; private set; }
        public ToboganQuestionState QuestionState { get; private set; }
        public ToboganPlayState PlayState { get; private set; }
        public ToboganResultGameState ResultState { get; private set; }
        
        public void ResetScore()
        {
            CurrentScoreRecord = 0;
            CurrentScore = 0;
        }

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
            pipesAnswerController.SetSignHidingProbability(ToboganConfiguration.Instance.Difficulty);

            IntroductionState = new ToboganIntroductionState(this);
            QuestionState = new ToboganQuestionState(this);
            PlayState = new ToboganPlayState(this);
            ResultState = new ToboganResultGameState(this);

            questionsManager = new QuestionsManager(this);
            questionsManager.onAnswered += OnResult;

            feedbackGraphics.Initialize(questionsManager);

            Physics.gravity = new Vector3(0, -80, 0);

            Context.GetStarsBarWidget().Show(STARS_1_THRESHOLD, STARS_2_THRESHOLD, STARS_3_THRESHOLD);
        }

        void OnResult(bool result)
        {
            Context.GetCheckmarkWidget().Show(result);

            if (result)
            {
                ++CurrentScore;
                if (CurrentScore > CurrentScoreRecord)
                    CurrentScoreRecord = CurrentScore;
            }
            else
            {
                CurrentScore = 0;
            }

            Context.GetStarsBarWidget().SetScore(CurrentScoreRecord);
        }
    }
}