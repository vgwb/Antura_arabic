namespace EA4S.Assessment
{
    /// <summary>
    /// This is the code specific to each Assessment (basically only initialization differs)
    /// </summary>
    public static class AssessmentFactory
    {
        private static AssessmentConfiguration configuration;
        private static IGameContext context;
        private static IAudioManager audioManager;

        public static IAssessment CreateLetterInWordAssessment()
        {
            Init();
            AssessmentConfiguration.Instance.PronunceQuestionWhenClicked = true;
            AssessmentConfiguration.Instance.PronunceAnswerWhenClicked = true;
            IAnswerChecker checker          = new DefaultAnswerChecker( context.GetCheckmarkWidget(), audioManager);
            IDragManager dragManager        = new DefaultDragManager( audioManager, checker);
            IQuestionDecorator questionDecorator = new PronunceQuestionDecorator();
            IQuestionGenerator generator    = new DefaultQuestionGenerator( configuration.Questions, QuestionType.LivingLetter);
            ILogicInjector injector         = new DefaultLogicInjector( dragManager, questionDecorator);
            IQuestionPlacer questionplacer  = new DefaultQuestionPlacer( audioManager);
            IAnswerPlacer answerPlacer      = new DefaultAnswerPlacer( audioManager);

            return new DefaultAssessment( answerPlacer, questionplacer, generator, injector, configuration, context);
        }

        public static IAssessment CreateLetterShapeAssessment()
        {
            Init();
            AssessmentConfiguration.Instance.PronunceQuestionWhenClicked = true;
            AssessmentConfiguration.Instance.PronunceAnswerWhenClicked = false;
            IAnswerChecker checker          = new DefaultAnswerChecker( context.GetCheckmarkWidget(), audioManager);
            IDragManager dragManager        = new DefaultDragManager( audioManager, checker);
            IQuestionDecorator questionDecorator = new PronunceAndFlipDecorator();
            IQuestionGenerator generator    = new DefaultQuestionGenerator( configuration.Questions, QuestionType.LivingLetter);
            ILogicInjector injector         = new DefaultLogicInjector( dragManager, questionDecorator);
            IQuestionPlacer questionplacer  = new DefaultQuestionPlacer( audioManager);
            IAnswerPlacer answerPlacer      = new DefaultAnswerPlacer( audioManager);

            return new DefaultAssessment( answerPlacer, questionplacer, generator, injector, configuration, context);
        }

        public static IAssessment CreateWordsWithLetterAssessment()
        {
            Init();
            AssessmentConfiguration.Instance.PronunceQuestionWhenClicked = true;
            AssessmentConfiguration.Instance.PronunceAnswerWhenClicked = true;
            IAnswerChecker checker          = new DefaultAnswerChecker( context.GetCheckmarkWidget(), audioManager);
            IDragManager dragManager        = new DefaultDragManager( audioManager, checker);
            IQuestionDecorator questionDecorator = new PronunceQuestionDecorator();
            IQuestionGenerator generator    = new DefaultQuestionGenerator( configuration.Questions, QuestionType.LivingLetter);
            ILogicInjector injector         = new DefaultLogicInjector( dragManager, questionDecorator);
            IQuestionPlacer questionplacer  = new DefaultQuestionPlacer( audioManager);
            IAnswerPlacer answerPlacer      = new DefaultAnswerPlacer( audioManager);

            return new DefaultAssessment( answerPlacer, questionplacer, generator, injector, configuration, context);
        }

        private static void Init()
        {
            // ARABIC SETTINGS
            AssessmentConfiguration.Instance.LocaleTextFlow = AssessmentConfiguration.TextFlow.RightToLeft;

            // Common Stuff
            TimeEngine.Instance.Clear();
            configuration = AssessmentConfiguration.Instance;
            context = configuration.Context;
            audioManager = configuration.Context.GetAudioManager();
        }
    }
}

