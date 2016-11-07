namespace EA4S.Tobogan
{
    public enum ToboganVariation : int
    {
        LetterInAWord = 1,
        SunMoon = 2
    }

    public class ToboganConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider Questions { get; set; }

        public float Difficulty { get; set; }
        public ToboganVariation Variation { get; set; }

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
            Questions = new SampleQuestionProvider();
            //PipeQuestions = new SunMoonQuestionProvider();
            //Variation = ToboganVariation.SunMoon;
            Variation = ToboganVariation.LetterInAWord;

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
