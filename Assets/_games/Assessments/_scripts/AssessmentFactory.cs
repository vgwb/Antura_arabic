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
            IGameContext context = configuration.Context;
            IAudioManager audioManager      = configuration.Context.GetAudioManager();
            IAnswerChecker checker = new DefaultAnswerChecker( context.GetCheckmarkWidget());
            IDragManager dragManager = new DefaultDragManager(audioManager, checker);
            IQuestionGenerator generator    = new DefaultQuestionGenerator( configuration.Questions, QuestionType.LivingLetter);
            ILogicInjector injector         = new DefaultLogicInjector( dragManager);
            IQuestionPlacer questionplacer  = new DefaultQuestionPlacer( audioManager);
            IAnswerPlacer answerPlacer      = new DefaultAnswerPlacer( audioManager);
            
            return null;
        }

        public static IAssessment CreateLetterShapeAssessment()
        {
            Init(); // common initialization stuff
            IAssessmentConfiguration configuration = AssessmentConfiguration.Instance;
            IGameContext context = configuration.Context;
            IAudioManager audioManager = configuration.Context.GetAudioManager();
            IAnswerChecker checker = new DefaultAnswerChecker( context.GetCheckmarkWidget());
            IDragManager dragManager = new DefaultDragManager( audioManager, checker);
            IQuestionGenerator generator = new DefaultQuestionGenerator( configuration.Questions, QuestionType.LivingLetter);
            ILogicInjector injector = new DefaultLogicInjector( dragManager);
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
