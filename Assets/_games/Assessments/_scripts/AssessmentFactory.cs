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
            //TODO: GET RID OF AUDIO MANAGERS => Configuration is singleton
            
            AssessmentConfiguration.Instance.PronunceQuestionWhenClicked = true;
            AssessmentConfiguration.Instance.PronunceAnswerWhenClicked = true;
            IAssessmentConfiguration configuration = AssessmentConfiguration.Instance;
            IGameContext context            = configuration.Context;
            IAudioManager audioManager      = configuration.Context.GetAudioManager();
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
            Init(); // common initialization stuff
            AssessmentConfiguration.Instance.PronunceQuestionWhenClicked = true;
            AssessmentConfiguration.Instance.PronunceAnswerWhenClicked = false;
            IAssessmentConfiguration configuration = AssessmentConfiguration.Instance;
            IGameContext context            = configuration.Context;
            IAudioManager audioManager      = configuration.Context.GetAudioManager();
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
            Init(); // common initialization stuff
            AssessmentConfiguration.Instance.PronunceQuestionWhenClicked = true;
            AssessmentConfiguration.Instance.PronunceAnswerWhenClicked = true;
            IAssessmentConfiguration configuration = AssessmentConfiguration.Instance;
            IGameContext context            = configuration.Context;
            IAudioManager audioManager      = configuration.Context.GetAudioManager();
            IAnswerChecker checker          = new DefaultAnswerChecker( context.GetCheckmarkWidget(), audioManager);
            IDragManager dragManager        = new DefaultDragManager( audioManager, checker);
            IQuestionDecorator questionDecorator = new PronunceQuestionDecorator();
            IQuestionGenerator generator    = new DefaultQuestionGenerator( configuration.Questions, QuestionType.LivingLetter);
            ILogicInjector injector         = new DefaultLogicInjector( dragManager, questionDecorator);
            IQuestionPlacer questionplacer  = new DefaultQuestionPlacer( audioManager);
            IAnswerPlacer answerPlacer      = new DefaultAnswerPlacer( audioManager);

            return new DefaultAssessment( answerPlacer, questionplacer, generator, injector, configuration, context);
        }

        //private LivingLetterFactory livingLetterFactory;

        private static void Init()
        {
            //ARABIC SETTINGS
            AssessmentConfiguration.Instance.LocaleTextFlow = AssessmentConfiguration.TextFlow.RightToLeft;
            TimeEngine.Instance.Clear();
        }
    }
}

