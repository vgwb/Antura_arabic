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
        public int SimultaneosQuestions { get; set; }
        public int Answers { get; set; }

        private int _rounds = 0;
        public int Rounds { get { return _rounds; } set { _rounds = value; } }

        public bool PronunceQuestionWhenClicked { get; set; }
        public bool PronunceAnswerWhenClicked { get; set; }


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

                default:
                    throw new NotImplementedException( "NotImplemented Yet!");
            }
        }

        private IQuestionBuilder Setup_WordsWithLetter_Builder()
        {
            SimultaneosQuestions = 2;
            snag.SetStartingFrom(0.5f);
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
            Answers = 2;// limited br screen size on 3:4 screen

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

                default:
                    throw new NotImplementedException( "NotImplemented Yet!");
            }
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
