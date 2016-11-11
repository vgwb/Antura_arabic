namespace EA4S.Test
{
    public class TestConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider Questions { get; set; }
        public float Difficulty { get; set; }

        /////////////////
        // Singleton Pattern
        static TestConfiguration instance;
        public static TestConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new TestConfiguration();
                return instance;
            }
        }
        /////////////////

        private TestConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            Context = new SampleGameContext();
            Difficulty = 0.5f;
        }

        public IQuestionBuilder SetupBuilder() {
            IQuestionBuilder builder = null;
            // TODO
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
