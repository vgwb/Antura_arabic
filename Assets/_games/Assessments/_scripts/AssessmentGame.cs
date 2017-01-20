using UnityEngine;

namespace EA4S.Assessment
{
    /// <summary>
    /// Extends MiniGame to interact with Antura's Hub, this class creates a different
    /// Assessment instance according to which assessment code was provided and
    /// Setup the MiniGameStates.
    /// </summary>
    public class AssessmentGame : MiniGame
    {
        [Header("Configuration")]
        public AssessmentCode assessmentCode;

        public AssessmentIntroState IntroState { get; private set; }
        public AssessmentGameState GameState { get; private set; }
        public AssessmentResultState ResultState { get; private set; }

        private Assessment assessment;

        private Assessment CreateConfiguredAssessment( AssessmentContext context)
        {
            AssessmentOptions.Reset();

            switch (AssessmentConfiguration.Instance.assessmentType)
            {
                case AssessmentCode.MatchLettersToWord:
                    return AssessmentFactory.CreateMatchLettersWordAssessment( context);

                case AssessmentCode.LetterShape:
                    return AssessmentFactory.CreateLetterShapeAssessment( context);

                case AssessmentCode.WordsWithLetter:
                    return AssessmentFactory.CreateWordsWithLetterAssessment( context);

                case AssessmentCode.SunMoonWord:
                    return AssessmentFactory.CreateSunMoonWordAssessment( context);

                case AssessmentCode.SunMoonLetter:
                    return AssessmentFactory.CreateSunMoonLetterAssessment( context);

                case AssessmentCode.QuestionAndReply:
                    return AssessmentFactory.CreateQuestionAndReplyAssessment( context);

                case AssessmentCode.SelectPronouncedWord:
                    return AssessmentFactory.CreatePronouncedWordAssessment( context);

                case AssessmentCode.SingularDualPlural:
                    return AssessmentFactory.CreateSingularDualPluralAssessment( context);

                case AssessmentCode.WordArticle:
                    return AssessmentFactory.CreateWordArticleAssessment( context);

                case AssessmentCode.MatchWordToImage:
                    return AssessmentFactory.CreateMatchWordToImageAssessment( context);

                case AssessmentCode.CompleteWord:
                    return AssessmentFactory.CreateCompleteWordAssessment( context);

                case AssessmentCode.OrderLettersOfWord:
                    return AssessmentFactory.CreateOrderLettersInWordAssessment( context);
            }

            return null;
        }

        protected override void OnInitialize( IGameContext gameContext)
        {
            AssessmentContext context = new AssessmentContext();
            context.Utils = gameContext;
            context.Game = this;
            assessment = CreateConfiguredAssessment( context);

            // We cannot Inject Widgets here, the references are in old scene
            // when we load a new scene the references are no longer valid!
            /*ResultState = new AssessmentResultState( this, context.DialogueManager);
            GameState = new AssessmentGameState( context.DragManager, assessment, ResultState, this);
            IntroState = new AssessmentIntroState( this, GameState, context.AudioManager);*/

            ResultState = new AssessmentResultState( this, context.DialogueManager);
            GameState = new AssessmentGameState( context.DragManager, assessment, ResultState, this);
            IntroState = new AssessmentIntroState( this, GameState, context.AudioManager);
        }

        protected override IGameState GetInitialState()
        {
            return IntroState;
        }

        protected override IGameConfiguration GetConfiguration()
        {
            return AssessmentConfiguration.Instance;
        }
    }
}

