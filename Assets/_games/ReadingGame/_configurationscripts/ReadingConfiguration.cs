namespace EA4S.Reading
{
    public class ReadingConfiguration : IGameConfiguration
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
        static ReadingConfiguration instance;
        public static ReadingConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new ReadingConfiguration();
                return instance;
            }
        }
        /////////////////

        private ReadingConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            Questions = new SampleQuestionProvider();

            Context = new SampleGameContext();
            Difficulty = 0.0f;
        }

        public IQuestionBuilder SetupBuilder() {
            IQuestionBuilder builder = null;
            // TODO
            return builder;
        }
    }
}
