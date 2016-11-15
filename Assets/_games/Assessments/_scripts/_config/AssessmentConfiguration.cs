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

        IQuestionBuilder builder = null;

        public IQuestionBuilder SetupBuilder()
        {
            /// TO M.PIROVANO: Dentro allo switch ci sono i metodi da implementare 
            /// Vedi anche il metodo SetupLearnRules()
            switch (assessmentType)
            {
                case AssessmentCode.LetterShape:
                    return Setup_LetterShape_Builder();

                case AssessmentCode.MatchLettersToWord:
                    return Setup_MatchLettersToWord_Builder();

                case AssessmentCode.WordsWithLetter:
                    return Setup_WordsWithLetter_Builder();

                default:
                    throw new ArgumentException( "NotImplemented Yet!");
            }
        }

        private IQuestionBuilder Setup_WordsWithLetter_Builder()
        {
            throw new NotImplementedException();
        }

        private IQuestionBuilder Setup_MatchLettersToWord_Builder()
        {
            throw new NotImplementedException();
        }

        private IQuestionBuilder Setup_LetterShape_Builder()
        {
            throw new NotImplementedException();
        }

        public MiniGameLearnRules SetupLearnRules()
        {
            /// TO M.PIROVANO: Dentro allo switch ci sono i metodi da implementare 
            /// Vedi anche il metodo SetupBuilder()
            switch (assessmentType)
            {
                case AssessmentCode.LetterShape:
                    return Setup_LetterShape_LearnRules();

                case AssessmentCode.MatchLettersToWord:
                    return Setup_MatchLettersToWord_LearnRules();

                case AssessmentCode.WordsWithLetter:
                    return Setup_WordsWithLetter_LearnRules();

                default:
                    throw new ArgumentException( "NotImplemented Yet!");
            }
        }

        private MiniGameLearnRules Setup_WordsWithLetter_LearnRules()
        {
            throw new NotImplementedException();
        }

        private MiniGameLearnRules Setup_MatchLettersToWord_LearnRules()
        {
            throw new NotImplementedException();
        }

        private MiniGameLearnRules Setup_LetterShape_LearnRules()
        {
            throw new NotImplementedException();
        }
    }
}
