namespace EA4S.FastCrowd
{
    public class FastCrowdConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider FindRightLetterQuestions { get; set; }
        #region Game configurations
        public float Difficulty { get; set; }
        public int Variation { get; set; }
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
            FindRightLetterQuestions = new SampleQuestionProvider();
            Context = new SampleGameContext();
            Difficulty = 0.5f;
            Variation = 1;
            BehaviourSettings = new LetterBehaviour.BehaviourSettings();
            MaxNumbOfWrongLettersNoise = 3;
            PlayTime = 90;
        }
    }
}
