// Author: Dario Oliveri ( https://github.com/Darelbi )

namespace EA4S.Assessment
{
    /// <summary>
    /// Create components for running a Assessment, components are specific
    /// according to Assessment type and some value tweak is also done here.
    /// </summary>
    public static class AssessmentFactory
    {
        private static AssessmentConfiguration configuration;
        private static IGameContext context;
        private static IAudioManager audioManager;
        private static ISubtitlesWidget subtitles;
        private static IDialogueManager dialogueManager;
        private static Db.LocalizationDataId gameDescription;
        private static readonly float letterSize = 1f * 3;
        private static readonly float wordSize = 1.5f * 3;
        private static readonly float sentenceSize = 2f * 3;
        private static int maxAnswers;
        private static int rounds;

        public static IAssessment CreateMatchWordToImageAssessment( IUpdater updater)
        {
            Init();
            AssessmentOptions.Instance.PronunceQuestionWhenClicked = true;
            AssessmentOptions.Instance.PronunceAnswerWhenClicked = false; // Do not pronunce name of a picture
            AssessmentOptions.Instance.ShowQuestionAsImage = true;
            IAnswerChecker checker = new DefaultAnswerChecker( context.GetCheckmarkWidget(), audioManager, dialogueManager);
            IDragManager dragManager = new DefaultDragManager( audioManager, checker);
            IQuestionDecorator questionDecorator = new PronunceImageDecorator();
            IQuestionGenerator generator = new DefaultQuestionGenerator( configuration.Questions);
            ILogicInjector injector = new DefaultLogicInjector( dragManager, questionDecorator);
            IQuestionPlacer questionplacer = new DefaultQuestionPlacer( audioManager, letterSize, wordSize);
            IAnswerPlacer answerPlacer = new DefaultAnswerPlacer( audioManager);

            gameDescription = Db.LocalizationDataId.Assessment_Match_Word_Image;
            updater.AddTimedUpdate( dragManager);

            return new DefaultAssessment(   answerPlacer, questionplacer, generator, injector,
                                            configuration, context, dialogueManager,
                                            gameDescription);
        }

        internal static IAssessment CreateOrderLettersInWordAssessment( IUpdater updater)
        {
            Init();
            AssessmentOptions.Instance.PronunceQuestionWhenClicked = true;
            AssessmentOptions.Instance.PronunceAnswerWhenClicked = true;
            AssessmentOptions.Instance.ShowQuestionAsImage = true;

            IDragManager dragManager = new SortingDragManager( audioManager, context.GetCheckmarkWidget());
            IQuestionDecorator questionDecorator = new PronunceImageDecorator();
            IQuestionGenerator generator = new ImageQuestionGenerator( configuration.Questions, false);
            ILogicInjector injector = new SortingLogicInjector( dragManager, questionDecorator);
            IQuestionPlacer questionplacer = new DefaultQuestionPlacer( audioManager, wordSize, letterSize);
            IAnswerPlacer answerPlacer = new InARowAnswerPlacer( audioManager, letterSize);

            gameDescription = Db.LocalizationDataId.Assessment_Order_Letters;
            updater.AddTimedUpdate( dragManager);

            return new DefaultAssessment(   answerPlacer, questionplacer, generator, injector,
                                            configuration, context, dialogueManager,
                                            gameDescription);
        }

        internal static IAssessment CreateCompleteWordAssessment( IUpdater updater)
        {
            Init();
            AssessmentOptions.Instance.PronunceQuestionWhenClicked = true;
            AssessmentOptions.Instance.PronunceAnswerWhenClicked = true;
            IAnswerChecker checker = new DefaultAnswerChecker( context.GetCheckmarkWidget(), audioManager, dialogueManager);
            IDragManager dragManager = new DefaultDragManager( audioManager, checker);
            IQuestionDecorator questionDecorator = new PronunceImageDecorator();
            IQuestionGenerator generator = new ImageQuestionGenerator( configuration.Questions, true);
            ILogicInjector injector = new DefaultLogicInjector( dragManager, questionDecorator);
            IQuestionPlacer questionplacer = new DefaultQuestionPlacer( audioManager, wordSize, letterSize, true);
            IAnswerPlacer answerPlacer = new DefaultAnswerPlacer( audioManager);

            gameDescription = Db.LocalizationDataId.Assessment_Select_Letter_Image;
            updater.AddTimedUpdate( dragManager);

            return new DefaultAssessment(   answerPlacer, questionplacer, generator, injector,
                                            configuration, context, dialogueManager,
                                            gameDescription);
        }

        public static IAssessment CreateMatchLettersWordAssessment( IUpdater updater)
        {
            Init();
            AssessmentOptions.Instance.PronunceQuestionWhenClicked = true;
            AssessmentOptions.Instance.PronunceAnswerWhenClicked = true;
            IAnswerChecker checker          = new DefaultAnswerChecker( context.GetCheckmarkWidget(), audioManager, dialogueManager);
            IDragManager dragManager        = new DefaultDragManager( audioManager, checker);
            IQuestionDecorator questionDecorator = new PronunceQuestionDecorator();
            IQuestionGenerator generator    = new DefaultQuestionGenerator( configuration.Questions);
            ILogicInjector injector         = new DefaultLogicInjector( dragManager, questionDecorator);
            IQuestionPlacer questionplacer  = new DefaultQuestionPlacer( audioManager, wordSize, letterSize);
            IAnswerPlacer answerPlacer      = new DefaultAnswerPlacer( audioManager);

            gameDescription = Db.LocalizationDataId.Assessment_Match_Letters_Words;
            updater.AddTimedUpdate( dragManager);

            return new DefaultAssessment(   answerPlacer, questionplacer, generator, injector,
                                            configuration, context, dialogueManager,
                                            gameDescription);
        }

        public static IAssessment CreateQuestionAndReplyAssessment( IUpdater updater)
        {
            Init();
            AssessmentOptions.Instance.PronunceQuestionWhenClicked = false; // Child should read question
            AssessmentOptions.Instance.PronunceAnswerWhenClicked = false; // Child shuold read answer
            IAnswerChecker checker = new DefaultAnswerChecker( context.GetCheckmarkWidget(), audioManager, dialogueManager);
            IDragManager dragManager = new DefaultDragManager( audioManager, checker);
            IQuestionDecorator questionDecorator = new PronunceQuestionDecorator();
            IQuestionGenerator generator = new DefaultQuestionGenerator( configuration.Questions);
            ILogicInjector injector = new DefaultLogicInjector( dragManager, questionDecorator);
            IQuestionPlacer questionplacer = new DefaultQuestionPlacer( audioManager, sentenceSize, sentenceSize);
            IAnswerPlacer answerPlacer = new DefaultAnswerPlacer( audioManager);

            gameDescription = Db.LocalizationDataId.Assessment_Match_Sentences;
            updater.AddTimedUpdate( dragManager);

            return new DefaultAssessment(   answerPlacer, questionplacer, generator, injector,
                                            configuration, context, dialogueManager,
                                            gameDescription);
        }

        public static IAssessment CreateSunMoonWordAssessment( IUpdater updater)
        {
            Init();
            AssessmentOptions.Instance.PronunceQuestionWhenClicked = true;
            AssessmentOptions.Instance.PronunceAnswerWhenClicked = true;
            IAnswerChecker checker = new DefaultAnswerChecker( context.GetCheckmarkWidget(), audioManager, dialogueManager);
            IDragManager dragManager = new DefaultDragManager( audioManager, checker);
            IQuestionDecorator questionDecorator = new PronunceQuestionDecorator();
            ICategoryProvider categoryProvider = new CategoryProvider( CategoryType.SunMoon);
            IQuestionGenerator generator = new CategoryQuestionGenerator(   configuration.Questions, 
                                                                            categoryProvider,
                                                                            maxAnswers, rounds);
            ILogicInjector injector = new DefaultLogicInjector( dragManager, questionDecorator);
            IQuestionPlacer questionplacer = new CategoryQuestionPlacer( audioManager, letterSize, wordSize);
            IAnswerPlacer answerPlacer = new DefaultAnswerPlacer( audioManager);

            gameDescription = Db.LocalizationDataId.Assessment_Classify_Words_Article;
            updater.AddTimedUpdate( dragManager);

            return new DefaultAssessment(   answerPlacer, questionplacer, generator, injector,
                                            configuration, context, dialogueManager,
                                            gameDescription);
        }

        public static IAssessment CreateSingularDualPluralAssessment( IUpdater updater)
        {
            Init();
            AssessmentOptions.Instance.PronunceQuestionWhenClicked = true;
            AssessmentOptions.Instance.PronunceAnswerWhenClicked = true;
            IAnswerChecker checker = new DefaultAnswerChecker( context.GetCheckmarkWidget(), audioManager, dialogueManager);
            IDragManager dragManager = new DefaultDragManager( audioManager, checker);
            IQuestionDecorator questionDecorator = new PronunceQuestionDecorator();
            ICategoryProvider categoryProvider = new CategoryProvider( CategoryType.SingularDualPlural);
            IQuestionGenerator generator = new CategoryQuestionGenerator(   configuration.Questions,
                                                                            categoryProvider,
                                                                            maxAnswers, rounds);
            ILogicInjector injector = new DefaultLogicInjector( dragManager, questionDecorator);
            IQuestionPlacer questionplacer = new CategoryQuestionPlacer( audioManager, letterSize, wordSize);
            IAnswerPlacer answerPlacer = new DefaultAnswerPlacer( audioManager);

            gameDescription = Db.LocalizationDataId.Assessment_Classify_Word_Nouns;
            updater.AddTimedUpdate( dragManager);

            return new DefaultAssessment(   answerPlacer, questionplacer, generator, injector,
                                            configuration, context, dialogueManager,
                                            gameDescription);
        }

        public static IAssessment CreateWordArticleAssessment( IUpdater updater)
        {
            Init();
            AssessmentOptions.Instance.PronunceQuestionWhenClicked = true;
            AssessmentOptions.Instance.PronunceAnswerWhenClicked = true;
            IAnswerChecker checker = new DefaultAnswerChecker( context.GetCheckmarkWidget(), audioManager, dialogueManager);
            IDragManager dragManager = new DefaultDragManager( audioManager, checker);
            IQuestionDecorator questionDecorator = new PronunceQuestionDecorator();
            ICategoryProvider categoryProvider = new CategoryProvider( CategoryType.WithOrWithoutArticle);
            IQuestionGenerator generator = new CategoryQuestionGenerator(   configuration.Questions,
                                                                            categoryProvider,
                                                                            maxAnswers, rounds);
            ILogicInjector injector = new DefaultLogicInjector( dragManager, questionDecorator);
            IQuestionPlacer questionplacer = new CategoryQuestionPlacer( audioManager, wordSize, wordSize);
            IAnswerPlacer answerPlacer = new DefaultAnswerPlacer( audioManager);

            gameDescription = Db.LocalizationDataId.Assessment_Classify_Word_Article;
            updater.AddTimedUpdate( dragManager);

            return new DefaultAssessment(   answerPlacer, questionplacer, generator, injector,
                                            configuration, context, dialogueManager,
                                            gameDescription);
        }

        public static IAssessment CreateSunMoonLetterAssessment( IUpdater updater)
        {
            Init();
            AssessmentOptions.Instance.PronunceQuestionWhenClicked = true;
            AssessmentOptions.Instance.PronunceAnswerWhenClicked = true;
            IAnswerChecker checker = new DefaultAnswerChecker( context.GetCheckmarkWidget(), audioManager, dialogueManager);
            IDragManager dragManager = new DefaultDragManager( audioManager, checker);
            IQuestionDecorator questionDecorator = new PronunceQuestionDecorator();
            ICategoryProvider categoryProvider = new CategoryProvider( CategoryType.SunMoon);
            IQuestionGenerator generator = new CategoryQuestionGenerator(   configuration.Questions,
                                                                            categoryProvider,
                                                                            maxAnswers, rounds);
            ILogicInjector injector = new DefaultLogicInjector( dragManager, questionDecorator);
            IQuestionPlacer questionplacer = new CategoryQuestionPlacer( audioManager, letterSize, letterSize);
            IAnswerPlacer answerPlacer = new DefaultAnswerPlacer( audioManager);

            gameDescription = Db.LocalizationDataId.Assessment_Classify_Letters_Article;
            updater.AddTimedUpdate( dragManager);

            return new DefaultAssessment(   answerPlacer, questionplacer, generator, injector,
                                            configuration, context, dialogueManager,
                                            gameDescription);
        }

        public static IAssessment CreateLetterShapeAssessment( IUpdater updater)
        {
            Init();
            AssessmentOptions.Instance.PronunceQuestionWhenClicked = true;
            AssessmentOptions.Instance.PronunceAnswerWhenClicked = false; // Child shuold identify the letter
            IAnswerChecker checker          = new DefaultAnswerChecker( context.GetCheckmarkWidget(), audioManager, dialogueManager);
            IDragManager dragManager        = new DefaultDragManager( audioManager, checker);
            IQuestionDecorator questionDecorator = new PronunceAndFlipDecorator();
            IQuestionGenerator generator    = new DefaultQuestionGenerator( configuration.Questions);
            ILogicInjector injector         = new DefaultLogicInjector( dragManager, questionDecorator);
            IQuestionPlacer questionplacer  = new DefaultQuestionPlacer( audioManager, letterSize, letterSize);
            IAnswerPlacer answerPlacer      = new DefaultAnswerPlacer( audioManager);

            gameDescription = Db.LocalizationDataId.Assessment_Select_Letter_Listen;
            updater.AddTimedUpdate( dragManager);

            return new DefaultAssessment(   answerPlacer, questionplacer, generator, injector, 
                                            configuration, context, dialogueManager,
                                            gameDescription);
        }

        public static IAssessment CreatePronouncedWordAssessment( IUpdater updater)
        {
            Init();
            AssessmentOptions.Instance.PronunceQuestionWhenClicked = true;
            AssessmentOptions.Instance.PronunceAnswerWhenClicked = false; // Child shuold identify the word
            IAnswerChecker checker = new DefaultAnswerChecker( context.GetCheckmarkWidget(), audioManager, dialogueManager);
            IDragManager dragManager = new DefaultDragManager( audioManager, checker);
            IQuestionDecorator questionDecorator = new PronunceAndFlipDecorator();
            IQuestionGenerator generator = new DefaultQuestionGenerator( configuration.Questions);
            ILogicInjector injector = new DefaultLogicInjector( dragManager, questionDecorator);
            IQuestionPlacer questionplacer = new DefaultQuestionPlacer( audioManager, wordSize, wordSize);
            IAnswerPlacer answerPlacer = new DefaultAnswerPlacer( audioManager);

            gameDescription = Db.LocalizationDataId.Assessment_Select_Word_Listen;
            updater.AddTimedUpdate( dragManager);

            return new DefaultAssessment(   answerPlacer, questionplacer, generator, injector,
                                            configuration, context, dialogueManager,
                                            gameDescription);
        }

        public static IAssessment CreateWordsWithLetterAssessment( IUpdater updater)
        {
            Init();
            AssessmentOptions.Instance.PronunceQuestionWhenClicked = true;
            AssessmentOptions.Instance.PronunceAnswerWhenClicked = true;
            IAnswerChecker checker          = new DefaultAnswerChecker( context.GetCheckmarkWidget(), audioManager, dialogueManager);
            IDragManager dragManager        = new DefaultDragManager( audioManager, checker);
            IQuestionDecorator questionDecorator = new PronunceQuestionDecorator();
            IQuestionGenerator generator    = new DefaultQuestionGenerator( configuration.Questions);
            ILogicInjector injector         = new DefaultLogicInjector( dragManager, questionDecorator);
            IQuestionPlacer questionplacer  = new DefaultQuestionPlacer( audioManager, letterSize, wordSize);
            IAnswerPlacer answerPlacer      = new DefaultAnswerPlacer( audioManager);

            gameDescription = Db.LocalizationDataId.Assessment_Select_Words;
            updater.AddTimedUpdate( dragManager);

            return new DefaultAssessment(   answerPlacer, questionplacer, generator, injector,
                                            configuration, context, dialogueManager,
                                            gameDescription);
        }

        /// <summary>
        /// Perform common initialization to reduce amount of code in creation of assessments.
        /// </summary>
        private static void Init()
        {
            // ARABIC SETTINGS
            AssessmentOptions.Instance.LocaleTextFlow = TextFlow.RightToLeft;

            configuration = AssessmentConfiguration.Instance;
            context = configuration.Context;
            audioManager = configuration.Context.GetAudioManager();
            subtitles = configuration.Context.GetSubtitleWidget();
            dialogueManager = new DialogueManager( audioManager, subtitles);
            rounds = configuration.Rounds;
            maxAnswers = configuration.Answers;
            AssessmentOptions.Instance.ShowQuestionAsImage = false;
            AssessmentOptions.Instance.PlayQuestionAudioAfterTutorial = false;
        }
    }
}

