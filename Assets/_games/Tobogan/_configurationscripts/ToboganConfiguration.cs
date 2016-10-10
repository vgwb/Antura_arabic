namespace EA4S.Tobogan
{
    public class ToboganConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider PipeQuestions { get; set; }

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
        static ToboganConfiguration instance;
        public static ToboganConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new ToboganConfiguration();
                return instance;
            }
        }
        /////////////////

        private ToboganConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            PipeQuestions = new SampleQuestionProvider();
            Context = new SampleGameContext();
            Difficulty = 0.5f;
        }

    }
}
