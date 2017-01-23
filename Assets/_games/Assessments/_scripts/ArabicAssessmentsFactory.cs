// Author: Dario Oliveri ( https://github.com/Darelbi )

namespace EA4S.Assessment
{
    /// <summary>
    /// Create components for running a Assessment, components are specific
    /// according to Assessment type and some value tweak is also done here.
    /// 
    /// Here Arabic assessments are configured, there is no point in 
    /// over-abstracting this one. The easiest way is to create another
    /// class LanguageXAssessmentsFactory using this file as template.
    /// </summary>
    public static class ArabicAssessmentsFactory
    {
        /// <summary>
        /// Configuration variables
        /// </summary>
        private static readonly float letterSize = 1f * 3;
        private static readonly float wordSize = 1.5f * 3;
        private static readonly float sentenceSize = 2f * 3;
        private static int maxAnswers;
        private static int rounds;

        public enum DragManagerType
        {
            Default,
            Sorting
        }

        public enum LogicInjectorType
        {
            Default,
            Sorting
        }

        public enum AnswerPlacerType
        {
            Line,
            Random
        }

        public static Assessment CreateMatchWordToImageAssessment( AssessmentContext context)
        {
            // Assessment Specific configuration.
            context.GameDescription = Db.LocalizationDataId.Assessment_Match_Word_Image;
            AssessmentOptions.Instance.PronunceQuestionWhenClicked = true;
            AssessmentOptions.Instance.PronunceAnswerWhenClicked = false; // Do not pronunce name of a picture
            AssessmentOptions.Instance.ShowQuestionAsImage = true;
            AssessmentOptions.Instance.QuestionSpawnedPlaySound = true; // pronunce the word of the image
            AssessmentOptions.Instance.QuestionAnsweredPlaySound = true;
            AssessmentOptions.Instance.PlayQuestionAudioAfterTutorial = true;
            AssessmentOptions.Instance.QuestionAnsweredFlip = false;

            // Get references from GameContext (utils)
            Init( context);

            // Instantiate the correct managers
            CreateManagers( context,
                            DragManagerType.Default,
                            LogicInjectorType.Default,
                            AnswerPlacerType.Random
                            );

            // Create the custom managers
            context.QuestionGenerator = new DefaultQuestionGenerator( context.Configuration.Questions);
            context.QuestionPlacer = new DefaultQuestionPlacer( context.AudioManager, letterSize, wordSize);

            // Build the assessment
            return CreateAssessment( context);
        }

        internal static Assessment CreateOrderLettersInWordAssessment( AssessmentContext context)
        {
            context.GameDescription = Db.LocalizationDataId.Assessment_Order_Letters;
            AssessmentOptions.Instance.PronunceQuestionWhenClicked = true;
            AssessmentOptions.Instance.PronunceAnswerWhenClicked = true;
            AssessmentOptions.Instance.ShowQuestionAsImage = true;
            AssessmentOptions.Instance.QuestionSpawnedPlaySound = true; // pronunce the word to sort
            AssessmentOptions.Instance.QuestionAnsweredPlaySound = true;
            AssessmentOptions.Instance.PlayQuestionAudioAfterTutorial = true;
            AssessmentOptions.Instance.QuestionAnsweredFlip = false;

            Init( context);

            CreateManagers( context,
                            DragManagerType.Sorting,
                            LogicInjectorType.Sorting,
                            AnswerPlacerType.Line
                            );


            context.QuestionGenerator = new ImageQuestionGenerator( context.Configuration.Questions, false);
            context.QuestionPlacer = new DefaultQuestionPlacer( context.AudioManager, wordSize, letterSize);

            return CreateAssessment( context);
        }

        internal static Assessment CreateCompleteWordAssessment( AssessmentContext context)
        {
            context.GameDescription = Db.LocalizationDataId.Assessment_Select_Letter_Image;
            AssessmentOptions.Instance.PronunceQuestionWhenClicked = true;
            AssessmentOptions.Instance.PronunceAnswerWhenClicked = true;
            AssessmentOptions.Instance.QuestionSpawnedPlaySound = true; // pronunce the complete word
            AssessmentOptions.Instance.QuestionAnsweredPlaySound = true;
            AssessmentOptions.Instance.ShowQuestionAsImage = false;
            AssessmentOptions.Instance.PlayQuestionAudioAfterTutorial = true;
            AssessmentOptions.Instance.QuestionAnsweredFlip = false;

            Init( context);

            CreateManagers( context,
                            DragManagerType.Default,
                            LogicInjectorType.Default,
                            AnswerPlacerType.Random
                            );

            context.QuestionGenerator = new ImageQuestionGenerator( context.Configuration.Questions, true);
            context.QuestionPlacer = new DefaultQuestionPlacer( context.AudioManager, wordSize, letterSize, true);

            return CreateAssessment( context);
        }

        public static Assessment CreateMatchLettersWordAssessment( AssessmentContext context)
        {
            context.GameDescription = Db.LocalizationDataId.Assessment_Match_Letters_Words;
            AssessmentOptions.Instance.PronunceQuestionWhenClicked = true;
            AssessmentOptions.Instance.PronunceAnswerWhenClicked = true;
            AssessmentOptions.Instance.ShowQuestionAsImage = false;
            AssessmentOptions.Instance.PlayQuestionAudioAfterTutorial = false;
            AssessmentOptions.Instance.QuestionSpawnedPlaySound = false;
            AssessmentOptions.Instance.QuestionAnsweredPlaySound = false;
            AssessmentOptions.Instance.QuestionAnsweredFlip = false;

            Init( context);

            CreateManagers( context,
                            DragManagerType.Default,
                            LogicInjectorType.Default,
                            AnswerPlacerType.Random
                            );

            context.QuestionGenerator = new DefaultQuestionGenerator( context.Configuration.Questions);
            context.QuestionPlacer = new DefaultQuestionPlacer( context.AudioManager, wordSize, letterSize);

            return CreateAssessment( context);
        }

        public static Assessment CreateQuestionAndReplyAssessment( AssessmentContext context)
        {
            context.GameDescription = Db.LocalizationDataId.Assessment_Match_Sentences;
            AssessmentOptions.Instance.PlayQuestionAudioAfterTutorial = false;
            AssessmentOptions.Instance.PronunceQuestionWhenClicked = false; // Child should read question
            AssessmentOptions.Instance.PronunceAnswerWhenClicked = false; // Child shuold read answer
            AssessmentOptions.Instance.ShowQuestionAsImage = false;
            AssessmentOptions.Instance.QuestionSpawnedPlaySound = false;
            AssessmentOptions.Instance.QuestionAnsweredPlaySound = false;
            AssessmentOptions.Instance.QuestionAnsweredFlip = false;

            Init( context);

            CreateManagers( context,
                            DragManagerType.Default,
                            LogicInjectorType.Default,
                            AnswerPlacerType.Random
                            );

            context.QuestionGenerator = new DefaultQuestionGenerator( context.Configuration.Questions);
            context.QuestionPlacer = new DefaultQuestionPlacer( context.AudioManager, sentenceSize, sentenceSize);

            return CreateAssessment( context);
        }

        public static Assessment CreateSunMoonWordAssessment( AssessmentContext context)
        {
            context.GameDescription = Db.LocalizationDataId.Assessment_Classify_Words_Article;
            AssessmentOptions.Instance.PronunceQuestionWhenClicked = true;
            AssessmentOptions.Instance.PronunceAnswerWhenClicked = true;
            AssessmentOptions.Instance.ShowQuestionAsImage = false;
            AssessmentOptions.Instance.PlayQuestionAudioAfterTutorial = false;
            AssessmentOptions.Instance.QuestionSpawnedPlaySound = false;
            AssessmentOptions.Instance.QuestionAnsweredPlaySound = false;
            AssessmentOptions.Instance.QuestionAnsweredFlip = false;

            Init( context);

            CreateManagers( context,
                            DragManagerType.Default,
                            LogicInjectorType.Default,
                            AnswerPlacerType.Random
                            );

            ArabicCategoryProvider categoryProvider = new ArabicCategoryProvider( CategoryType.SunMoon);
            context.QuestionGenerator = new CategoryQuestionGenerator( context.Configuration.Questions, 
                                                                            categoryProvider,
                                                                            maxAnswers, rounds);
            context.QuestionPlacer = new CategoryQuestionPlacer( context.AudioManager, letterSize, wordSize);

            return CreateAssessment( context);
        }

        public static Assessment CreateSingularDualPluralAssessment( AssessmentContext context)
        {
            context.GameDescription = Db.LocalizationDataId.Assessment_Classify_Word_Nouns;
            AssessmentOptions.Instance.PronunceQuestionWhenClicked = true;
            AssessmentOptions.Instance.PronunceAnswerWhenClicked = true;
            AssessmentOptions.Instance.ShowQuestionAsImage = false;
            AssessmentOptions.Instance.PlayQuestionAudioAfterTutorial = false;
            AssessmentOptions.Instance.QuestionSpawnedPlaySound = false;
            AssessmentOptions.Instance.QuestionAnsweredPlaySound = false;
            AssessmentOptions.Instance.QuestionAnsweredFlip = false;

            Init( context);

            CreateManagers( context,
                            DragManagerType.Default,
                            LogicInjectorType.Default,
                            AnswerPlacerType.Random
                            );

            ArabicCategoryProvider categoryProvider = new ArabicCategoryProvider( CategoryType.SingularDualPlural);
            context.QuestionGenerator = new CategoryQuestionGenerator( context.Configuration.Questions,
                                                                            categoryProvider,
                                                                            maxAnswers, rounds);
            context.QuestionPlacer = new CategoryQuestionPlacer( context.AudioManager, letterSize, wordSize);

            return CreateAssessment( context);
        }

        public static Assessment CreateWordArticleAssessment( AssessmentContext context)
        {
            context.GameDescription = Db.LocalizationDataId.Assessment_Classify_Word_Article;
            AssessmentOptions.Instance.PronunceQuestionWhenClicked = true;
            AssessmentOptions.Instance.PronunceAnswerWhenClicked = true;
            AssessmentOptions.Instance.ShowQuestionAsImage = false;
            AssessmentOptions.Instance.PlayQuestionAudioAfterTutorial = false;
            AssessmentOptions.Instance.QuestionSpawnedPlaySound = false;
            AssessmentOptions.Instance.QuestionAnsweredPlaySound = false;
            AssessmentOptions.Instance.QuestionAnsweredFlip = false;

            Init( context);

            CreateManagers( context,
                            DragManagerType.Default,
                            LogicInjectorType.Default,
                            AnswerPlacerType.Random
                            );

            ArabicCategoryProvider categoryProvider = new ArabicCategoryProvider( CategoryType.WithOrWithoutArticle);
            context.QuestionGenerator = new CategoryQuestionGenerator( context.Configuration.Questions,
                                                                            categoryProvider,
                                                                            maxAnswers, rounds);
            context.QuestionPlacer = new CategoryQuestionPlacer( context.AudioManager, wordSize, wordSize);

            return CreateAssessment( context);
        }

        public static Assessment CreateSunMoonLetterAssessment( AssessmentContext context)
        {
            context.GameDescription = Db.LocalizationDataId.Assessment_Classify_Letters_Article;
            AssessmentOptions.Instance.PronunceQuestionWhenClicked = true;
            AssessmentOptions.Instance.PronunceAnswerWhenClicked = true;

            AssessmentOptions.Instance.ShowQuestionAsImage = false;
            AssessmentOptions.Instance.PlayQuestionAudioAfterTutorial = false;
            AssessmentOptions.Instance.QuestionSpawnedPlaySound = false;
            AssessmentOptions.Instance.QuestionAnsweredPlaySound = false;
            AssessmentOptions.Instance.QuestionAnsweredFlip = false;

            Init( context);

            CreateManagers( context,
                            DragManagerType.Default,
                            LogicInjectorType.Default,
                            AnswerPlacerType.Random
                            );

            ArabicCategoryProvider categoryProvider = new ArabicCategoryProvider( CategoryType.SunMoon);
            context.QuestionGenerator = new CategoryQuestionGenerator( context.Configuration.Questions,
                                                                            categoryProvider,
                                                                            maxAnswers, rounds);
            context.QuestionPlacer = new CategoryQuestionPlacer( context.AudioManager, letterSize, letterSize);

            return CreateAssessment( context);
        }

        public static Assessment CreateLetterShapeAssessment( AssessmentContext context)
        {
            context.GameDescription = Db.LocalizationDataId.Assessment_Select_Letter_Listen;
            AssessmentOptions.Instance.PronunceQuestionWhenClicked = true;
            AssessmentOptions.Instance.PronunceAnswerWhenClicked = false; // Child shuold identify the letter
            AssessmentOptions.Instance.QuestionSpawnedPlaySound = true; // pronunce the word to sort
            AssessmentOptions.Instance.QuestionAnsweredPlaySound = true;
            AssessmentOptions.Instance.QuestionAnsweredFlip = true;
            AssessmentOptions.Instance.ShowQuestionAsImage = false;
            AssessmentOptions.Instance.PlayQuestionAudioAfterTutorial = true;

            Init( context);

            CreateManagers( context,
                            DragManagerType.Default,
                            LogicInjectorType.Default,
                            AnswerPlacerType.Random
                            );

            context.QuestionGenerator = new DefaultQuestionGenerator( context.Configuration.Questions);
            context.QuestionPlacer = new DefaultQuestionPlacer( context.AudioManager, letterSize, letterSize);

            return CreateAssessment( context);
        }

        public static Assessment CreatePronouncedWordAssessment( AssessmentContext context)
        {
            context.GameDescription = Db.LocalizationDataId.Assessment_Select_Word_Listen;
            AssessmentOptions.Instance.PronunceQuestionWhenClicked = true;
            AssessmentOptions.Instance.PronunceAnswerWhenClicked = false; // Child should identify the word
            AssessmentOptions.Instance.QuestionSpawnedPlaySound = true; // pronunce the word to sort
            AssessmentOptions.Instance.QuestionAnsweredPlaySound = true;
            AssessmentOptions.Instance.QuestionAnsweredFlip = true;
            AssessmentOptions.Instance.ShowQuestionAsImage = false;
            AssessmentOptions.Instance.PlayQuestionAudioAfterTutorial = true;

            Init( context);

            CreateManagers( context,
                            DragManagerType.Default,
                            LogicInjectorType.Default,
                            AnswerPlacerType.Random
                            );

            context.QuestionGenerator = new DefaultQuestionGenerator( context.Configuration.Questions);
            context.QuestionPlacer = new DefaultQuestionPlacer( context.AudioManager, wordSize, wordSize);

            return CreateAssessment( context);
        }

        public static Assessment CreateWordsWithLetterAssessment( AssessmentContext context)
        {
            context.GameDescription = Db.LocalizationDataId.Assessment_Select_Words;
            AssessmentOptions.Instance.PronunceQuestionWhenClicked = true;
            AssessmentOptions.Instance.PronunceAnswerWhenClicked = true;
            AssessmentOptions.Instance.ShowQuestionAsImage = false;
            AssessmentOptions.Instance.PlayQuestionAudioAfterTutorial = false;
            AssessmentOptions.Instance.QuestionSpawnedPlaySound = false;
            AssessmentOptions.Instance.QuestionAnsweredPlaySound = false;
            AssessmentOptions.Instance.QuestionAnsweredFlip = false;

            Init( context);

            CreateManagers( context,
                            DragManagerType.Default,
                            LogicInjectorType.Default,
                            AnswerPlacerType.Random
                            );

            context.QuestionGenerator = new DefaultQuestionGenerator( context.Configuration.Questions);
            context.QuestionPlacer = new DefaultQuestionPlacer( context.AudioManager, letterSize, wordSize);

            return CreateAssessment( context);
        }

        /// <summary>
        /// Perform common initialization
        /// </summary>
        private static void Init( AssessmentContext context)
        {
            // ARABIC SETTINGS
            AssessmentOptions.Instance.LocaleTextFlow = TextFlow.RightToLeft;

            context.Configuration = AssessmentConfiguration.Instance;
            context.Utils = AssessmentConfiguration.Instance.Context;
            context.AudioManager = context.Utils.GetAudioManager();
            context.Subtitles = context.Utils.GetSubtitleWidget();
            context.CheckmarkWidget = context.Utils.GetCheckmarkWidget();
            context.DialogueManager = new AssessmentDialogues( context.AudioManager,
                                                               context.Subtitles,
                                                               context.GameDescription);

            context.AnswerChecker = new AnswerChecker( context.CheckmarkWidget,
                                                       context.AudioManager,
                                                       context.DialogueManager);

            rounds = AssessmentConfiguration.Instance.Rounds;
            maxAnswers = AssessmentConfiguration.Instance.Answers;
        }

        /// <summary>
        /// Create Assessment from context
        /// </summary>
        /// <param name="context"> managers used to configure the game</param>
        private static Assessment CreateAssessment( AssessmentContext context)
        {
            return new Assessment( context.AnswerPlacer, context.QuestionPlacer, context.QuestionGenerator,
                                   context.LogicInjector, context.Configuration, context.Utils,
                                   context.DialogueManager);
        }

        /// <summary>
        /// Create managers depending on Assessment type
        /// </summary>
        private static void CreateManagers(
                AssessmentContext context,
                DragManagerType dragManager,
                LogicInjectorType logicInjector,
                AnswerPlacerType answerPlacer)
        {
            if (dragManager == DragManagerType.Default)
                context.DragManager = new DefaultDragManager( context.AudioManager, context.AnswerChecker);
            else
                context.DragManager = new SortingDragManager( context.AudioManager, context.CheckmarkWidget);

            if (logicInjector == LogicInjectorType.Default)
                context.LogicInjector = new DefaultLogicInjector( context.DragManager);
            else
                context.LogicInjector = new SortingLogicInjector( context.DragManager);

            if (answerPlacer == AnswerPlacerType.Line)
                context.AnswerPlacer = new LineAnswerPlacer( context.AudioManager, letterSize);
            else
                context.AnswerPlacer = new RandomAnswerPlacer( context.AudioManager);
        }
    }
}

