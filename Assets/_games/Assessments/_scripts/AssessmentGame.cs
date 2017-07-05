using Antura.MinigamesCommon;
using UnityEngine;

namespace Antura.Assessment
{
    /// <summary>
    /// Extends MiniGame to interact with Antura's Hub, this class creates a different
    /// Assessment instance according to which assessment code was provided and
    /// Setup the MiniGameStates.
    /// </summary>
    public class AssessmentGame : MiniGame
    {
        [HideInInspector]
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
                    return ArabicAssessmentsFactory.CreateMatchLettersWordAssessment( context);

                case AssessmentCode.LetterForm:
                    return ArabicAssessmentsFactory.CreateLetterFormAssessment( context);

                case AssessmentCode.WordsWithLetter:
                    return ArabicAssessmentsFactory.CreateWordsWithLetterAssessment( context);

                case AssessmentCode.SunMoonWord:
                    return ArabicAssessmentsFactory.CreateSunMoonWordAssessment( context);

                case AssessmentCode.SunMoonLetter:
                    return ArabicAssessmentsFactory.CreateSunMoonLetterAssessment( context);

                case AssessmentCode.QuestionAndReply:
                    return ArabicAssessmentsFactory.CreateQuestionAndReplyAssessment( context);

                case AssessmentCode.SelectPronouncedWord:
                    return ArabicAssessmentsFactory.CreatePronouncedWordAssessment( context);

                case AssessmentCode.SingularDualPlural:
                    return ArabicAssessmentsFactory.CreateSingularDualPluralAssessment( context);

                case AssessmentCode.WordArticle:
                    return ArabicAssessmentsFactory.CreateWordArticleAssessment( context);

                case AssessmentCode.MatchWordToImage:
                    return ArabicAssessmentsFactory.CreateMatchWordToImageAssessment( context);

                case AssessmentCode.CompleteWord:
                    return ArabicAssessmentsFactory.CreateCompleteWordAssessment( context);

                case AssessmentCode.OrderLettersOfWord:
                    return ArabicAssessmentsFactory.CreateOrderLettersInWordAssessment( context);

                case AssessmentCode.CompleteWord_Form:
                    return ArabicAssessmentsFactory.CreateCompleteWord_FormAssessment( context);

                case AssessmentCode.MatchLettersToWord_Form:
                    return ArabicAssessmentsFactory.CreateMatchLettersToWord_FormAssessment( context);
            }

            return null;
        }

        protected override void OnInitialize( IGameContext gameContext)
        {
            AssessmentContext context = new AssessmentContext();
            context.Utils = gameContext;
            context.Game = this;
            assessment = CreateConfiguredAssessment( context);

            ResultState = new AssessmentResultState( this, context.AudioManager);
            GameState = new AssessmentGameState( context.DragManager, assessment, ResultState, this);
            IntroState = new AssessmentIntroState( this, GameState, context.AudioManager);
        }

        protected override IState GetInitialState()
        {
            return IntroState;
        }

        protected override IGameConfiguration GetConfiguration()
        {
            return AssessmentConfiguration.Instance;
        }
    }
}

