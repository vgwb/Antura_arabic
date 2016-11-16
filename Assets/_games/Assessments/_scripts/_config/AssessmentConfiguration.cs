using System;

namespace EA4S.Assessment
{
    public class AssessmentConfiguration : IAssessmentConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider RandomQuestions { get; set; }
        public float Difficulty { get; set; }
        public int SimultaneosQuestions { get; set; }
        public int Rounds { get; set; }
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

        public string Description { get { return "Missing description"; } private set { } }

        public IQuestionProvider Questions { get; set; }

        /////////////////

        private AssessmentConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            Questions = new SampleQuestionProvider();
            Context = new SampleGameContext();
            SimultaneosQuestions = 2;
            Rounds = 2;
        }

        public IQuestionBuilder SetupBuilder()
        {
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
            return new WordsWithLetterQuestionBuilder(nPacks: 10, nCorrect: 5, nWrong: 5);
        }

        private IQuestionBuilder Setup_MatchLettersToWord_Builder()
        {
            return new LettersInWordQuestionBuilder(nPacks: 10, useAllCorrectLetters: true, nWrong: 5);
        }

        private IQuestionBuilder Setup_LetterShape_Builder()
        {
            return new RandomLettersQuestionBuilder(nPacks: 10, nCorrect:1, firstCorrectIsQuestion:true, nWrong: 5, packListHistory: Teacher.PackListHistory.ForceAllDifferent, wrongAnswersPackListHistory: Teacher.PackListHistory.ForceAllDifferent);
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
