namespace EA4S.HideAndSeek
{
    public class HideAndSeekConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }

        public IQuestionProvider Questions { get; set; }

        public float Difficulty { get; set; }

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
            Context = new SampleGameContext();
            Questions = new SampleQuestionProvider();
            Difficulty = 0.5f;
        }

        public IQuestionBuilder SetupBuilder() {
            IQuestionBuilder builder = null;

            int nPacks = 10;
            int nCorrect = 5;

            var builderParams = new Teacher.QuestionBuilderParameters();
            builder = new RandomLettersQuestionBuilder(nPacks, nCorrect, parameters: builderParams);

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
