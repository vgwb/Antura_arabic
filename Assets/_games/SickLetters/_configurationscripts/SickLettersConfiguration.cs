namespace EA4S.SickLetters
{
    public class SickLettersConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }
        public float Difficulty { get; set; }
        public IQuestionProvider Questions { get; set; }

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
            Context = new SampleGameContext();
            Questions = new SampleQuestionProvider();
            Difficulty = 0.5f;
        }

        public IQuestionBuilder SetupBuilder() {
            IQuestionBuilder builder = null;

            int nPacks = 10;
            int nCorrect = 1;
            int nWrong = 5;

            builder = new RandomLettersQuestionBuilder(nPacks, nCorrect, nWrong);

            return builder;
        }
    }
}
