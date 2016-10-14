namespace EA4S.FastCrowd
{
    public enum FastCrowdVariation
    {
        Spelling = 1,
        Words = 2,
        Letter = 3,
        Counting = 4,
        Alphabet = 5
    }

    public class FastCrowdConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }

        // For Spelling, Words, Counting (words of the numbers)
        public ILivingLetterDataProvider WordsProvider { get; set; }

        // For Letter, Alphabet
        public ILivingLetterDataProvider LetterProvider { get; set; }

        #region Game configurations
        public float Difficulty { get; set; }
        public FastCrowdVariation Variation { get; set; }
        public float PlayTime { get; set; }
        public int MaxNumbOfWrongLettersNoise { get; set; }
        #endregion
        #region Behaviour configurations
        public LetterBehaviour.BehaviourSettings BehaviourSettings { get; set; }
        #endregion

        /////////////////
        // Singleton Pattern
        static FastCrowdConfiguration instance;
        public static FastCrowdConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new FastCrowdConfiguration();
                return instance;
            }
        }
        /////////////////

        private FastCrowdConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            WordsProvider = new SampleWordProvider();
            LetterProvider = new SampleLetterProvider();
            Context = new SampleGameContext();
            Difficulty = 0.5f;
            Variation = FastCrowdVariation.Words;
            BehaviourSettings = new LetterBehaviour.BehaviourSettings();
            MaxNumbOfWrongLettersNoise = 3;
            PlayTime = 90;
        }
    }
}
