using EA4S.MinigamesAPI;
using EA4S.MinigamesAPI.Sample;
using EA4S.MinigamesCommon;
using EA4S.Teacher;

namespace EA4S.Minigames.HideAndSeek
{
    public class HideAndSeekConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }

        public IQuestionProvider Questions { get; set; }

        public float Difficulty { get; set; }
        public bool PerformTutorial { get; set; }

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
            PerformTutorial = true;
        }

        public IQuestionBuilder SetupBuilder() {
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
