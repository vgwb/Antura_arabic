
namespace EA4S.Assessment
{
    public class AssessmentGameState : IGameState
    {
        private AssessmentGame assessmentGame;
        private IAssessment assessment;
        private IUpdater updater;

        public AssessmentGameState( AssessmentGame assessmentGame)
        {
            this.assessmentGame = assessmentGame;
            // Updater allows to update non-monobehaviours
            updater = Updater.Instance;

            assessment = GetAssessment();
        }

        private IAssessment GetAssessment()
        {
            // We wire updater at composition root level to keep other code clean.
            switch (AssessmentConfiguration.Instance.assessmentType)
            {
                case AssessmentCode.MatchLettersToWord:
                    return AssessmentFactory.CreateMatchLettersWordAssessment( updater);

                case AssessmentCode.LetterShape:
                    return AssessmentFactory.CreateLetterShapeAssessment( updater);

                case AssessmentCode.WordsWithLetter:
                    return AssessmentFactory.CreateWordsWithLetterAssessment( updater);

                case AssessmentCode.SunMoonWord:
                    return AssessmentFactory.CreateSunMoonWordAssessment( updater);

                case AssessmentCode.SunMoonLetter:
                    return AssessmentFactory.CreateSunMoonLetterAssessment( updater);

                case AssessmentCode.QuestionAndReply:
                    return AssessmentFactory.CreateQuestionAndReplyAssessment( updater);

                case AssessmentCode.SelectPronouncedWord:
                    return AssessmentFactory.CreatePronouncedWordAssessment( updater);

                case AssessmentCode.SingularDualPlural:
                    return AssessmentFactory.CreateSingularDualPluralAssessment( updater);

                case AssessmentCode.WordArticle:
                    return AssessmentFactory.CreateWordArticleAssessment( updater);

                case AssessmentCode.MatchWordToImage:
                    return AssessmentFactory.CreateMatchWordToImageAssessment( updater);

                case AssessmentCode.CompleteWord:
                    return AssessmentFactory.CreateCompleteWordAssessment( updater);

                case AssessmentCode.OrderLettersOfWord:
                    return AssessmentFactory.CreateOrderLettersInWordAssessment( updater);
            }

            return null;
        }

        public void EnterState()
        {
            assessment.StartGameSession(SetNextState);
        }

        public void SetNextState()
        {
            assessmentGame.SetCurrentState( assessmentGame.ResultState);
        }

        public void ExitState()
        {
            updater.Clear();
            updater = null;
        }

        public void Update( float delta)
        {
            updater.UpdateDelta( delta);
        }

        public void UpdatePhysics( float delta)
        {

        }
    }
}
