namespace EA4S.Egg
{
    public class EggConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }
        public ILivingLetterDataProvider QuestionProvider { get; set; }
        public float Difficulty { get; set; }

        /////////////////
        // Singleton Pattern
        static EggConfiguration instance;
        public static EggConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new EggConfiguration();
                return instance;
            }
        }
        /////////////////

        private EggConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            Context = new SampleGameContext();
            Difficulty = 0.8f;
            QuestionProvider = new SampleEggQuestionProvider(Difficulty);
        }

    }
}