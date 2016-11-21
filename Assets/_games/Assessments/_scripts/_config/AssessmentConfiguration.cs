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
            
            if(questionProvider == null)
            {
                switch (assessmentType)
                {
                    case AssessmentCode.LetterShape:
                        Debug.Log( "Created LetterShape_TestProvider");
                        return questionProvider = new LetterShape_TestProvider( 2, 2, 3);

                    case AssessmentCode.WordsWithLetter:
                        Debug.Log("Created WordsWithLetterProvider_Tester");
                        return questionProvider = new WordsWithLetterProvider_Tester( rounds:2, simultaneos:2, correct:3, wrong:2);

                    case AssessmentCode.MatchLettersToWord:
                        Debug.Log("Created WordsWithLetterProvider_Tester");
                        return questionProvider = new MatchLettersToWordProvider_Tester( rounds: 2, simultaneos: 2, correct: 3, wrong: 2);

                    default:
                        Debug.LogWarning( "Created SampleQuestionProvider");
                        return questionProvider = new SampleQuestionProvider();
                }
            }
            return questionProvider;
        }

        internal void SetupDefault(AssessmentCode code)
        {
            if (Instance.assessmentType == AssessmentCode.Unsetted)
            {
                Instance.assessmentType = code;

                switch (assessmentType)
                {
                    case AssessmentCode.LetterShape:
                        SimultaneosQuestions = 1;
                        Rounds = 3;
                        break;

                    case AssessmentCode.MatchLettersToWord:
                        SimultaneosQuestions = 1;
                        Rounds = 2;
                        break;

                    case AssessmentCode.WordsWithLetter:
                        SimultaneosQuestions = 2;
                        Rounds = 1;
                        break;

                    default:
                        throw new NotImplementedException( "NotImplemented Yet!");
                }
            }
        }

        public float Difficulty { get; set; }
        public int SimultaneosQuestions { get; set; }

        private int _rounds = 0;
        public int Rounds { get { return _rounds; } set { _rounds = value; Debug.Log("Setted Rounds:" + value); } }

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

        public string Description { get { return "Missing description"; } private set { } }

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
        /// <returns></returns>
        public IQuestionBuilder SetupBuilder()
        {
            snag = new DifficultyRegulation( Difficulty);

            switch (assessmentType)
            {
                case AssessmentCode.LetterShape:
                    return Setup_LetterShape_Builder();

                case AssessmentCode.MatchLettersToWord:
                    return Setup_MatchLettersToWord_Builder();

                case AssessmentCode.WordsWithLetter:
                    return Setup_WordsWithLetter_Builder();

                default:
                    throw new NotImplementedException( "NotImplemented Yet!");
            }
        }

        private IQuestionBuilder Setup_WordsWithLetter_Builder()
        {
            SimultaneosQuestions = snag.Increase( 1,2);
            Rounds = snag.Increase( 2, 4);
            return new WordsWithLetterQuestionBuilder( 

                SimultaneosQuestions*Rounds,// Total Answers
                snag.Decrease( 3, 2),       // Correct Answers
                snag.Increase( 1, 4));      // Wrong Answers
        }

        private IQuestionBuilder Setup_MatchLettersToWord_Builder()
        {
            SimultaneosQuestions = 1;
            Rounds = snag.Increase( 2, 3);
            return new LettersInWordQuestionBuilder(

                SimultaneosQuestions * Rounds,  // Total Answers
                snag.Decrease( 3, 2),            // CorrectAnswers
                snag.Increase( 1, 4),            // WrongAnswers
                useAllCorrectLetters: false);
        }

        private IQuestionBuilder Setup_LetterShape_Builder()
        {
            SimultaneosQuestions = 1;
            Rounds = snag.Decrease( 4, 1);      // We assume letter shapes are just a basic thing so we don't insist

            var builderParams = new QuestionBuilderParameters();
            builderParams.correctChoicesHistory = Teacher.PackListHistory.ForceAllDifferent;
            builderParams.wrongChoicesHistory = Teacher.PackListHistory.ForceAllDifferent;

            return new RandomLettersQuestionBuilder(
                SimultaneosQuestions * Rounds,  // Total Answers
                1,                              // CorrectAnswers
                snag.Increase(3, 6),           // WrongAnswers
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

                default:
                    throw new NotImplementedException( "NotImplemented Yet!");
            }
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
