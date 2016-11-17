using System;
using UnityEngine;

namespace EA4S.Assessment
{
    public class AssessmentConfiguration : IAssessmentConfiguration
    {
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
                        throw new NotImplementedException("NotImplemented Yet!");
                }
            }
        }

        public float Difficulty { get; set; }
        public int SimultaneosQuestions { get; set; }
        public int Rounds { get; set; }

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

        }

        /// <summary>
        /// This is called by MiniGameAPI to create QuestionProvider, that means that if I start game
        /// from debug scene, I need a custom test Provider.
        /// </summary>
        /// <returns></returns>
        public IQuestionBuilder SetupBuilder()
        {
            Debug.Log( "2) SetupQuestionBuilder");
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
            return new WordsWithLetterQuestionBuilder( nPacks: 2, nCorrect: 3, nWrong: 3);
        }

        private IQuestionBuilder Setup_MatchLettersToWord_Builder()
        {
            return new LettersInWordQuestionBuilder( nPacks: 2, useAllCorrectLetters: true, nWrong: 2);
        }

        private IQuestionBuilder Setup_LetterShape_Builder()
        {
            return new RandomLettersQuestionBuilder( nPacks: 2, nCorrect:1, firstCorrectIsQuestion:true, nWrong: 3, packListHistory: Teacher.PackListHistory.ForceAllDifferent, wrongAnswersPackListHistory: Teacher.PackListHistory.ForceAllDifferent);
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
