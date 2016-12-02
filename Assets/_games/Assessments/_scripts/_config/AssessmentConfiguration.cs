using System;
using UnityEngine;

namespace EA4S.Assessment
{
    public class AssessmentConfiguration : IAssessmentConfiguration
    {
        public enum TextFlow
        {
            LeftToRight,
            RightToLeft
        }

        public TextFlow LocaleTextFlow { get; set; }

        // Game configuration
        public IGameContext Context { get; set; }

        private IQuestionProvider questionProvider;
        public IQuestionProvider Questions
        {
            get
            {
                return GetQuestionProvider();
            }
            set
            {
                Debug.Log("AssessmentConfiguration: Injected Provider");
                questionProvider = value;
            }
        }

        private IQuestionProvider GetQuestionProvider()
        {
            return questionProvider;
        }

        public float Difficulty { get; set; }
        public int SimultaneosQuestions { get; set; } // should match category numbers
        public int Answers { get; set; } // number of answers in category questions

        private int _rounds = 0;
        public int Rounds { get { return _rounds; } set { _rounds = value; } }

        public bool PronunceQuestionWhenClicked { get; set; }
        public bool PronunceAnswerWhenClicked { get; set; }
        public bool ShowQuestionAsImage { get; set; }


        public AssessmentCode assessmentType = AssessmentCode.Unsetted;

        /////////////////
        // Singleton Pattern
        static AssessmentConfiguration instance;
        public static AssessmentConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new AssessmentConfiguration();
                return instance;
            }
        }
        /////////////////

        public string Description { get { return "Missing description AND audio"; } private set { } }

        private AssessmentConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            questionProvider = null;
            Context = new SampleGameContext();
            LocaleTextFlow = TextFlow.RightToLeft;

        }

        private DifficultyRegulation snag;

        /// <summary>
        /// This is called by MiniGameAPI to create QuestionProvider, that means that if I start game
        /// from debug scene, I need a custom test Provider.
        /// </summary>
        /// <returns>Custom question data for the assessment</returns>
        public IQuestionBuilder SetupBuilder()
        {
            // Testing question builders
            snag = new DifficultyRegulation( Difficulty);

            switch (assessmentType)
            {
                case AssessmentCode.LetterShape:
                    return Setup_LetterShape_Builder();

                case AssessmentCode.MatchLettersToWord:
                    return Setup_MatchLettersToWord_Builder();

                case AssessmentCode.WordsWithLetter:
                    return Setup_WordsWithLetter_Builder();

                case AssessmentCode.SunMoonWord:
                    return Setup_SunMoonWords_Builder();

                case AssessmentCode.SunMoonLetter:
                    return Setup_SunMoonLetter_Builder();

                case AssessmentCode.QuestionAndReply:
                    return Setup_QuestionAnReply_Builder();

                case AssessmentCode.SelectPronouncedWord:
                    return Setup_SelectPronuncedWord_Builder();

                case AssessmentCode.SingularDualPlural:
                    return Setup_SingularDualPlural_Builder();

                case AssessmentCode.WordArticle:
                    return Setup_WordArticle_Builder();

                case AssessmentCode.MatchWordToImage:
                    return Setup_MatchWordToImage_Builder();

                case AssessmentCode.CompleteWord:
                    return Setup_CompleteWord_Builder();

                case AssessmentCode.OrderLettersOfWord:
                    return Setup_OrderLettersOfWord_Builder();

                default:
                    throw new NotImplementedException( "NotImplemented Yet!");
            }
        }

        private IQuestionBuilder Setup_OrderLettersOfWord_Builder()
        {
            SimultaneosQuestions = 1;
            Rounds = 3;

            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.wordFilters.requireDrawings = true;
            
            return new LettersInWordQuestionBuilder(
                Rounds,
                2,
                useAllCorrectLetters: true,
                parameters: builderParams
                );
        }

        private IQuestionBuilder Setup_CompleteWord_Builder()
        {
            SimultaneosQuestions = 1;
            Rounds = 3;

            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.correctChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wrongChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wrongSeverity = Teacher.SelectionSeverity.MayRepeatIfNotEnough;
            builderParams.useJourneyForWrong = true;
            builderParams.wordFilters.requireDrawings = true;

            return new LettersInWordQuestionBuilder(

                SimultaneosQuestions * Rounds,  // Total Answers
                1,                              // Always one!
                snag.Increase(3, 5),            // WrongAnswers
                useAllCorrectLetters: false,
                parameters: builderParams);
        }

        private IQuestionBuilder Setup_MatchWordToImage_Builder()
        {
            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.correctChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wrongChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wrongSeverity = Teacher.SelectionSeverity.MayRepeatIfNotEnough;
            builderParams.useJourneyForWrong = false;
            builderParams.wordFilters.requireDrawings = true;
            SimultaneosQuestions = 1;
            Rounds = 3;
            int nCorrect = 1;
            int nWrong = snag.Increase( 2, 4);

            return new RandomWordsQuestionBuilder(
                SimultaneosQuestions * Rounds,
                nCorrect,
                nWrong,
                firstCorrectIsQuestion: true,
                parameters: builderParams);
        }

        private IQuestionBuilder Setup_WordArticle_Builder()
        {
            SimultaneosQuestions = 2;
            Rounds = 3;
            Answers = 2;
            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.correctChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wordFilters.excludeArticles = false;

            return new WordsByArticleQuestionBuilder(
                Answers * Rounds * 3,
                builderParams);
        }

        private IQuestionBuilder Setup_SingularDualPlural_Builder()
        {
            SimultaneosQuestions = 3;
            Rounds = 3;
            Answers = 2;
            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.correctChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wordFilters.excludePluralDual = false;

            return new WordsByFormQuestionBuilder(
                SimultaneosQuestions*Rounds*4,
                builderParams);
        }

        private IQuestionBuilder Setup_SelectPronuncedWord_Builder()
        {
            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.correctChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wrongChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wrongSeverity = Teacher.SelectionSeverity.MayRepeatIfNotEnough;
            builderParams.useJourneyForWrong = false;
            SimultaneosQuestions = 1;
            Rounds = 3;
            int nCorrect = 1;
            int nWrong = snag.Increase(2, 4);
            return new RandomWordsQuestionBuilder(
                SimultaneosQuestions*Rounds,
                nCorrect,
                nWrong,
                firstCorrectIsQuestion: true,
                parameters: builderParams);
        }

        private IQuestionBuilder Setup_QuestionAnReply_Builder()
        {
            SimultaneosQuestions = 1;
            Rounds = 3;
            int nCorrect = 1;
            int nWrongs = snag.Increase( 2, 4);

            return new  PhraseQuestionsQuestionBuilder(
                        SimultaneosQuestions * Rounds, // totale questions
                        nCorrect,
                        nWrongs     // wrong additional answers
                );
        }

        private IQuestionBuilder Setup_SunMoonLetter_Builder()
        {
            SimultaneosQuestions = 2;
            Rounds = 3;
            Answers = 2;

            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.correctChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;

            return new LettersBySunMoonQuestionBuilder( 
                        SimultaneosQuestions * Rounds * 3,
                        builderParams
            );
        }

        private IQuestionBuilder Setup_WordsWithLetter_Builder()
        {
            SimultaneosQuestions = 2;
            snag.SetStartingFrom( 0.5f);
            Rounds = 3;

            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.correctChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wrongChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wrongSeverity = Teacher.SelectionSeverity.MayRepeatIfNotEnough;
            builderParams.useJourneyForWrong = false;

            return new WordsWithLetterQuestionBuilder( 

                SimultaneosQuestions*Rounds,    // Total Answers
                1,                              // Correct Answers
                snag.Increase( 1, 2),           // Wrong Answers
                parameters: builderParams
                );     

        }

        private IQuestionBuilder Setup_SunMoonWords_Builder()
        {
            SimultaneosQuestions = 2;
            Rounds = 3;
            Answers = 2;

            return new WordsBySunMoonQuestionBuilder( SimultaneosQuestions * Rounds * 2);
        }

        private IQuestionBuilder Setup_MatchLettersToWord_Builder()
        {
            SimultaneosQuestions = 1;
            Rounds = 3;

            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.correctChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wrongChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wrongSeverity = Teacher.SelectionSeverity.MayRepeatIfNotEnough;
            builderParams.useJourneyForWrong = false;

            return new LettersInWordQuestionBuilder(

                SimultaneosQuestions * Rounds,   // Total Answers
                snag.Decrease( 3, 2),            // CorrectAnswers
                snag.Increase( 1, 4),            // WrongAnswers
                useAllCorrectLetters: false,
                parameters: builderParams);
        }

        private IQuestionBuilder Setup_LetterShape_Builder()
        {
            SimultaneosQuestions = 1;
            Rounds = 3;

            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.correctChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wrongChoicesHistory = Teacher.PackListHistory.RepeatWhenFull;
            builderParams.wrongSeverity = Teacher.SelectionSeverity.MayRepeatIfNotEnough;
            builderParams.useJourneyForWrong = false;

            return new RandomLettersQuestionBuilder(
                SimultaneosQuestions * Rounds,  // Total Answers
                1,                              // CorrectAnswers
                snag.Increase( 3, 6),           // WrongAnswers
                firstCorrectIsQuestion:true,
                parameters:builderParams);
        }

        public MiniGameLearnRules SetupLearnRules()
        {
            switch (assessmentType)
            {
                case AssessmentCode.LetterShape:
                    return Setup_LetterShape_LearnRules();

                case AssessmentCode.MatchLettersToWord:
                    return Setup_MatchLettersToWord_LearnRules();

                case AssessmentCode.WordsWithLetter:
                    return Setup_WordsWithLetter_LearnRules();

                case AssessmentCode.SunMoonWord:
                    return Setup_SunMoonWords_LearnRules();

                case AssessmentCode.SunMoonLetter:
                    return Setup_SunMoonLetter_LearnRules();

                case AssessmentCode.QuestionAndReply:
                    return Setup_QuestionAnReply_LearnRules();

                case AssessmentCode.SelectPronouncedWord:
                    return Setup_SelectPronuncedWord_LearnRules();

                case AssessmentCode.SingularDualPlural:
                    return Setup_SingularDualPlural_LearnRules();

                case AssessmentCode.WordArticle:
                    return Setup_WordArticle_LearnRules();

                case AssessmentCode.MatchWordToImage:
                    return Setup_MatchWordToImage_LearnRules();

                case AssessmentCode.CompleteWord:
                    return Setup_CompleteWord_LearnRules();

                case AssessmentCode.OrderLettersOfWord:
                    return Setup_OrderLettersOfWord_LearnRules();

                default:
                    throw new NotImplementedException( "NotImplemented Yet!");
            }
        }

        private MiniGameLearnRules Setup_OrderLettersOfWord_LearnRules()
        {
            return new MiniGameLearnRules();
        }

        private MiniGameLearnRules Setup_CompleteWord_LearnRules()
        {
            return new MiniGameLearnRules();
        }

        private MiniGameLearnRules Setup_MatchWordToImage_LearnRules()
        {
            return new MiniGameLearnRules();
        }

        private MiniGameLearnRules Setup_WordArticle_LearnRules()
        {
            return new MiniGameLearnRules();
        }

        private MiniGameLearnRules Setup_SingularDualPlural_LearnRules()
        {
            return new MiniGameLearnRules();
        }

        private MiniGameLearnRules Setup_SelectPronuncedWord_LearnRules()
        {
            return new MiniGameLearnRules();
        }

        private MiniGameLearnRules Setup_QuestionAnReply_LearnRules()
        {
            return new MiniGameLearnRules();
        }

        private MiniGameLearnRules Setup_SunMoonLetter_LearnRules()
        {
            return new MiniGameLearnRules();
        }

        private MiniGameLearnRules Setup_SunMoonWords_LearnRules()
        {
            return new MiniGameLearnRules();
        }

        private MiniGameLearnRules Setup_WordsWithLetter_LearnRules()
        {
            return new MiniGameLearnRules();
        }

        private MiniGameLearnRules Setup_MatchLettersToWord_LearnRules()
        {
            return new MiniGameLearnRules();
        }

        private MiniGameLearnRules Setup_LetterShape_LearnRules()
        {
            return new MiniGameLearnRules();
        }

    }
}
