namespace EA4S.Assessment
{
    /// <summary>
    /// This is the code specific to each Assessment (basically only initialization differs)
    /// </summary>
    public static class AssessmentFactory
    {
        public static IAssessment CreateLetterInWordAssessment()
        {
            Init(); // common initialization stuff
            IAssessmentConfiguration configuration = AssessmentConfiguration.Instance;
            IAudioManager audioManager      = configuration.Context.GetAudioManager();
            IQuestionGenerator generator    = new DefaultQuestionGenerator( configuration.Questions, QuestionType.LivingLetter);
            ILogicInjector injector         = new DefaultLogicInjector();
            IQuestionPlacer questionplacer  = new DefaultQuestionPlacer( audioManager);
            IAnswerPlacer answerPlacer      = new DefaultAnswerPlacer( audioManager);
            //            return new DefaultAssessment();
            return null;
        }

        public static IAssessment CreateLetterShapeAssessment()
        {
            Init(); // common initialization stuff
            IAssessmentConfiguration configuration = AssessmentConfiguration.Instance;
            IAudioManager audioManager = configuration.Context.GetAudioManager();
            IGameContext context = configuration.Context;
            IQuestionGenerator generator = new DefaultQuestionGenerator( configuration.Questions, QuestionType.LivingLetter);
            ILogicInjector injector = new DefaultLogicInjector();
            IQuestionPlacer questionplacer = new DefaultQuestionPlacer( audioManager);
            IAnswerPlacer answerPlacer = new DefaultAnswerPlacer( audioManager);

            return new DefaultAssessment( answerPlacer, questionplacer, generator, injector, configuration, context);
        }

        public static IAssessment CreateWordsWithLetterAssessment()
        {
            Init(); // common initialization stuff
            IAssessmentConfiguration configuration = AssessmentConfiguration.Instance;
            var generator = new DefaultQuestionGenerator( configuration.Questions, QuestionType.LivingLetter);

            //            return new DefaultAssessment();
            return null;
        }

        //private LivingLetterFactory livingLetterFactory;

        private static void Init()
        {
            TimeEngine.Instance.Clear();
        }
    }
}
