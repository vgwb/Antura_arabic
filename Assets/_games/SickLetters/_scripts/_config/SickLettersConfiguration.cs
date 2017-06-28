using EA4S.MinigamesAPI;
using EA4S.MinigamesCommon;
using EA4S.Teacher;

namespace EA4S.Minigames.SickLetters
{
    public class SickLettersConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }
        public float Difficulty { get; set; }
        public bool PerformTutorial { get; set; }
        public IQuestionProvider Questions { get; set; }
        //public SickLettersQuestionProvider SickLettersQuestions { get; set; }

        //public SickLettersConfiguration Questions { get; set; }

        /////////////////
        // Singleton Pattern
        static SickLettersConfiguration instance;
        public static SickLettersConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new SickLettersConfiguration();
                return instance;
            }
        }
        /////////////////

        private SickLettersConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            Context = new MinigamesGameContext(MiniGameCode.SickLetters, System.DateTime.Now.Ticks.ToString());
            Questions = new SickLettersQuestionProvider();
            PerformTutorial = true;
            //SickLettersQuestions = new SickLettersQuestionProvider();
            Difficulty = 0.1f;
            EA4S.Teacher.ConfigAI.verboseTeacher = true;
        }

        public IQuestionBuilder SetupBuilder() {
            IQuestionBuilder builder = null;

            int nPacks = 20;
            int nCorrect = 1;
            int nWrong = 0;

            var builderParams = new Teacher.QuestionBuilderParameters();
            builder = new RandomLettersQuestionBuilder(nPacks, nCorrect, nWrong, parameters:builderParams);
            
            return builder;
        }

        public MiniGameLearnRules SetupLearnRules()
        {
            var rules = new MiniGameLearnRules();
            // example: a.minigameVoteSkewOffset = 1f;
            return rules;
        }

    }
}
