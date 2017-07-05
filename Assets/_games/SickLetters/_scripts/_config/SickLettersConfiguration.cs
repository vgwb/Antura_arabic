using Antura.MinigamesAPI;
using Antura.MinigamesCommon;
using Antura.Teacher;

namespace Antura.Minigames.SickLetters
{
    public enum SickLettersVariation 
    {
        Default = MiniGameCode.SickLetters,
    }

    public class SickLettersConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }

        public float Difficulty { get; set; }
        public bool TutorialEnabled { get; set; }
        public IQuestionProvider Questions { get; set; }
        public SickLettersVariation Variation { get; set; }

        public void SetMiniGameCode(MiniGameCode code)
        {
            Variation = (SickLettersVariation) code;
        }

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
            TutorialEnabled = true;
            //SickLettersQuestions = new SickLettersQuestionProvider();
            Difficulty = 0.1f;
            ConfigAI.verboseTeacher = true;
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
