using System;
using TMPro;
using UnityEngine;

namespace EA4S.Tobogan
{
    public class ToboganGame : MiniGame
    {
        public PipesAnswerController pipesAnswerController;
        public GameObject questionLivingLetterPrefab;
        public FixedHeightShadow shadowPrefab;
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

        public IQuestionProvider SunMoonQuestions { get; set; }

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

        bool tutorial;
        public bool showTutorial { get { if (tutorial) { tutorial = false; return true; } else return false; } }

        public ToboganQuestionState QuestionState { get; private set; }
        public ToboganPlayState PlayState { get; private set; }
        public ToboganResultGameState ResultState { get; private set; }
        public ToboganTutorialState TutorialState { get; private set; }
        
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
            return QuestionState;
        }

        protected override void OnInitialize(IGameContext context)
        {
            tutorial = true;

            pipesAnswerController.SetSignHidingProbability(ToboganConfiguration.Instance.Difficulty);
            SunMoonQuestions = new SunMoonTutorialQuestionProvider(ToboganConfiguration.Instance.Questions);

            QuestionState = new ToboganQuestionState(this);
            PlayState = new ToboganPlayState(this);
            ResultState = new ToboganResultGameState(this);
            TutorialState = new ToboganTutorialState(this);

            questionsManager = new QuestionsManager(this);

            feedbackGraphics.Initialize();
        }

        public void OnResult(bool result)
        {
            Context.GetCheckmarkWidget().Show(result);
            feedbackGraphics.OnResult(result);

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

            Context.GetOverlayWidget().SetStarsScore(CurrentScoreRecord);
        }

        public void InitializeOverlayWidget()
        {
            Context.GetOverlayWidget().Initialize(true, true, false);
            Context.GetOverlayWidget().SetStarsThresholds(STARS_1_THRESHOLD, STARS_2_THRESHOLD, STARS_3_THRESHOLD);
        }

        public override Vector3 GetGravity()
        {
            return Vector3.up * (-80);
        }
    }
}