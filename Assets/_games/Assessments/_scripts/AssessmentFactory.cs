using System;

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
        private static ISubtitlesWidget subtitles;
        private static IDialogueManager dialogueManager;
        private static Db.LocalizationDataId gameDescription;
        private static readonly float letterSize = 1f * 3;
        private static readonly float wordSize = 1.5f * 3;
        private static readonly float sentenceSize = 2f * 3;

        public static IAssessment CreateLetterInWordAssessment()
        {
            Init();
            AssessmentConfiguration.Instance.PronunceQuestionWhenClicked = true;
            AssessmentConfiguration.Instance.PronunceAnswerWhenClicked = true;
            IAnswerChecker checker          = new DefaultAnswerChecker( context.GetCheckmarkWidget(), audioManager, dialogueManager);
            IDragManager dragManager        = new DefaultDragManager( audioManager, checker);
            IQuestionDecorator questionDecorator = new PronunceQuestionDecorator();
            IQuestionGenerator generator    = new DefaultQuestionGenerator( configuration.Questions);
            ILogicInjector injector         = new DefaultLogicInjector( dragManager, questionDecorator);
            IQuestionPlacer questionplacer  = new DefaultQuestionPlacer( audioManager, wordSize, letterSize);
            IAnswerPlacer answerPlacer      = new DefaultAnswerPlacer( audioManager);

            gameDescription = Db.LocalizationDataId.Assessment_Match_Letters_Words;

            return new DefaultAssessment(answerPlacer, questionplacer, generator, injector,
                                            configuration, context, dialogueManager,
                                            gameDescription);
        }

        public static IAssessment CreateQuestionAndReplyAssessment()
        {
            Init();
            AssessmentConfiguration.Instance.PronunceQuestionWhenClicked = false;
            AssessmentConfiguration.Instance.PronunceAnswerWhenClicked = false;
            IAnswerChecker checker = new DefaultAnswerChecker( context.GetCheckmarkWidget(), audioManager, dialogueManager);
            IDragManager dragManager = new DefaultDragManager( audioManager, checker);
            IQuestionDecorator questionDecorator = new PronunceQuestionDecorator();
            IQuestionGenerator generator = new DefaultQuestionGenerator( configuration.Questions);
            ILogicInjector injector = new DefaultLogicInjector( dragManager, questionDecorator);
            IQuestionPlacer questionplacer = new DefaultQuestionPlacer( audioManager, sentenceSize, sentenceSize);
            IAnswerPlacer answerPlacer = new DefaultAnswerPlacer( audioManager);

            gameDescription = Db.LocalizationDataId.Assessment_Match_Sentences;

            return new DefaultAssessment(   answerPlacer, questionplacer, generator, injector,
                                            configuration, context, dialogueManager,
                                            gameDescription);
        }

        public static IAssessment CreateSunMoonWordAssessment()
        {
            Init();
            AssessmentConfiguration.Instance.PronunceQuestionWhenClicked = true;
            AssessmentConfiguration.Instance.PronunceAnswerWhenClicked = true;
            int rounds = AssessmentConfiguration.Instance.Rounds;
            int simult = AssessmentConfiguration.Instance.SimultaneosQuestions;
            IAnswerChecker checker = new DefaultAnswerChecker( context.GetCheckmarkWidget(), audioManager, dialogueManager);
            IDragManager dragManager = new DefaultDragManager( audioManager, checker);
            IQuestionDecorator questionDecorator = new PronunceQuestionDecorator();
            ICategoryProvider categoryProvider = new CategoryProvider( CategoryType.SunMoon);
            IQuestionGenerator generator = new CategoryQuestionGenerator(   configuration.Questions, 
                                                                            categoryProvider,
                                                                            simult, rounds);
            ILogicInjector injector = new DefaultLogicInjector( dragManager, questionDecorator);
            IQuestionPlacer questionplacer = new CategoryQuestionPlacer( audioManager, letterSize, wordSize);
            IAnswerPlacer answerPlacer = new DefaultAnswerPlacer( audioManager);

            gameDescription = Db.LocalizationDataId.Assessment_Classify_Words_Article;

            return new DefaultAssessment(   answerPlacer, questionplacer, generator, injector,
                                            configuration, context, dialogueManager,
                                            gameDescription);
        }

        public static IAssessment CreateSunMoonLetterAssessment()
        {
            Init();
            AssessmentConfiguration.Instance.PronunceQuestionWhenClicked = true;
            AssessmentConfiguration.Instance.PronunceAnswerWhenClicked = true;
            int rounds = AssessmentConfiguration.Instance.Rounds;
            int simult = AssessmentConfiguration.Instance.SimultaneosQuestions;
            IAnswerChecker checker = new DefaultAnswerChecker( context.GetCheckmarkWidget(), audioManager, dialogueManager);
            IDragManager dragManager = new DefaultDragManager( audioManager, checker);
            IQuestionDecorator questionDecorator = new PronunceQuestionDecorator();
            ICategoryProvider categoryProvider = new CategoryProvider( CategoryType.SunMoon);
            IQuestionGenerator generator = new CategoryQuestionGenerator(   configuration.Questions,
                                                                            categoryProvider,
                                                                            simult, rounds);
            ILogicInjector injector = new DefaultLogicInjector( dragManager, questionDecorator);
            IQuestionPlacer questionplacer = new CategoryQuestionPlacer( audioManager, letterSize, letterSize);
            IAnswerPlacer answerPlacer = new DefaultAnswerPlacer( audioManager);

            gameDescription = Db.LocalizationDataId.Assessment_Classify_Letters_Article;

            return new DefaultAssessment(   answerPlacer, questionplacer, generator, injector,
                                            configuration, context, dialogueManager,
                                            gameDescription);
        }

        public static IAssessment CreateLetterShapeAssessment()
        {
            Init();
            AssessmentConfiguration.Instance.PronunceQuestionWhenClicked = true;
            AssessmentConfiguration.Instance.PronunceAnswerWhenClicked = false;
            IAnswerChecker checker          = new DefaultAnswerChecker( context.GetCheckmarkWidget(), audioManager, dialogueManager);
            IDragManager dragManager        = new DefaultDragManager( audioManager, checker);
            IQuestionDecorator questionDecorator = new PronunceAndFlipDecorator();
            IQuestionGenerator generator    = new DefaultQuestionGenerator( configuration.Questions);
            ILogicInjector injector         = new DefaultLogicInjector( dragManager, questionDecorator);
            IQuestionPlacer questionplacer  = new DefaultQuestionPlacer( audioManager, letterSize, letterSize);
            IAnswerPlacer answerPlacer      = new DefaultAnswerPlacer( audioManager);

            gameDescription = Db.LocalizationDataId.Assessment_Select_Letter_Listen;

            return new DefaultAssessment(   answerPlacer, questionplacer, generator, injector, 
                                            configuration, context, dialogueManager,
                                            gameDescription);
        }

        public static IAssessment CreateWordsWithLetterAssessment()
        {
            Init();
            AssessmentConfiguration.Instance.PronunceQuestionWhenClicked = true;
            AssessmentConfiguration.Instance.PronunceAnswerWhenClicked = true;
            IAnswerChecker checker          = new DefaultAnswerChecker( context.GetCheckmarkWidget(), audioManager, dialogueManager);
            IDragManager dragManager        = new DefaultDragManager( audioManager, checker);
            IQuestionDecorator questionDecorator = new PronunceQuestionDecorator();
            IQuestionGenerator generator    = new DefaultQuestionGenerator( configuration.Questions);
            ILogicInjector injector         = new DefaultLogicInjector( dragManager, questionDecorator);
            IQuestionPlacer questionplacer  = new DefaultQuestionPlacer( audioManager, letterSize, wordSize);
            IAnswerPlacer answerPlacer      = new DefaultAnswerPlacer( audioManager);

            gameDescription = Db.LocalizationDataId.Assessment_Select_Words;

            return new DefaultAssessment(answerPlacer, questionplacer, generator, injector,
                                            configuration, context, dialogueManager,
                                            gameDescription);
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
            subtitles = configuration.Context.GetSubtitleWidget();
            dialogueManager = new DialogueManager( audioManager, subtitles);
        }
    }
}

