namespace EA4S.MixedLetters
{
    public class MixedLettersConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }
        public float Difficulty { get; set; }
        public IQuestionProvider MixedLettersQuestions { get; set; }

        /////////////////
        // Singleton Pattern
        static MixedLettersConfiguration instance;
        public static MixedLettersConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new MixedLettersConfiguration();
                return instance;
            }
        }
        /////////////////

        private MixedLettersConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            MixedLettersQuestions = new SampleQuestionProvider();
            Context = new SampleGameContext();
            Difficulty = 0.5f;
        }
    }
}
