namespace EA4S.ReadingGame
{
    public class ReadingGameConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider Questions { get; set; }

        public float Difficulty { get; set; }

        public int GetDiscreteDifficulty(int maximum)
        {
            int d = (int) Difficulty * (maximum + 1);

            if (d > maximum)
                return maximum;
            return d;
        }

        /////////////////
        // Singleton Pattern
        static ReadingGameConfiguration instance;
        public static ReadingGameConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new ReadingGameConfiguration();
                return instance;
            }
        }
        /////////////////

        private ReadingGameConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            Questions = new SampleReadingGameQuestionProvider();

            Context = new SampleGameContext();
            Difficulty = 0.0f;
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
