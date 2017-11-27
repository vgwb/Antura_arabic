using Antura.LivingLetters;
using Antura.LivingLetters.Sample;
using Antura.Teacher;

namespace Antura.Minigames.HideAndSeek
{
    public enum HideAndSeekVariation
    {
        Default = MiniGameCode.HideSeek
    }

    public class HideAndSeekConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider Questions { get; set; }

        public float Difficulty { get; set; }
        public bool TutorialEnabled { get; set; }
        public HideAndSeekVariation Variation { get; set; }

        public void SetMiniGameCode(MiniGameCode code)
        {
            Variation = (HideAndSeekVariation)code;
        }

        /////////////////
        // Singleton Pattern
        static HideAndSeekConfiguration instance;
        public static HideAndSeekConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new HideAndSeekConfiguration();
                return instance;
            }
        }
        /////////////////

        private HideAndSeekConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            Context = new MinigamesGameContext(MiniGameCode.HideSeek, System.DateTime.Now.Ticks.ToString());
            Questions = new SampleQuestionProvider();
            Difficulty = 0.5f;
            TutorialEnabled = true;
        }

        public IQuestionBuilder SetupBuilder()
        {
            IQuestionBuilder builder = null;

            int nPacks = 10;
            int nCorrect = 1;
            int nWrong = 6;

            var builderParams = new Teacher.QuestionBuilderParameters();
            builder = new RandomLettersQuestionBuilder(nPacks, nCorrect, nWrong, parameters: builderParams);

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
