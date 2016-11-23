namespace EA4S.DancingDots {
    public enum DancingDotsVariation : int {
        V_1 = 1,
    }

    public class DancingDotsConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider Questions { get; set; }
		public DancingDotsQuestionProvider DancingDotsQuestions { get; set; }


        #region Game configurations
        public float Difficulty { get; set; }
        public DancingDotsVariation Variation { get; set; }
        #endregion

        /////////////////
        // Singleton Pattern
        static DancingDotsConfiguration instance;
        public static DancingDotsConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new DancingDotsConfiguration();
                return instance;
            }
        }
        /////////////////

        private DancingDotsConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
			Context = new SampleGameContext();

            Variation = DancingDotsVariation.V_1;
			Questions = new DancingDotsQuestionProvider();
			DancingDotsQuestions = new DancingDotsQuestionProvider();
//            Difficulty = 0.5f;
        }

        #region external configuration call
        public static void SetConfiguration(float _difficulty, int _variation)
        {
            instance = new DancingDotsConfiguration()
            {
                Difficulty = _difficulty,
                Variation = (DancingDotsVariation)_variation,
            };
        }
        #endregion

        public IQuestionBuilder SetupBuilder()
        {
            IQuestionBuilder builder = null;

            int nPacks = 6;
            int nCorrect = 1;
            int nWrong = 0;

            builder = new RandomLettersQuestionBuilder(nPacks, nCorrect, nWrong);

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
