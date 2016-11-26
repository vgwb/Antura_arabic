namespace EA4S.Scanner 
{
    public class ScannerConfiguration : IGameConfiguration 
	{
        // Game configuration
		public IGameContext Context { get; set; }
		public IQuestionProvider Questions { get; set; }
		public bool gameActive = true;
		public float beltSpeed = 1f;

        #region Game configurations
        public float Difficulty { get; set; }
        #endregion
//		public LetterBehaviour.BehaviourSettings BehaviourSettings { get; set; }

        /////////////////
        // Singleton Pattern
        static ScannerConfiguration instance;
        public static ScannerConfiguration Instance 
		{
            get 
			{
                if (instance == null)
                    instance = new ScannerConfiguration();
                return instance;
            }
        }
        /////////////////

        private ScannerConfiguration() {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE

            Questions = new SampleQuestionProvider();

            Context = new SampleGameContext();
            Difficulty = 0.5f;
        }

        #region external configuration call
        public static void SetConfiguration(float _difficulty, int _variation) {
            instance = new ScannerConfiguration() {
                Difficulty = _difficulty,
            };
        }
        #endregion

        public IQuestionBuilder SetupBuilder() {

            IQuestionBuilder builder = null;

			int nPacks = 7; // One Extra for tutorial
			int nCorrect = 1;
			int nWrong = 4;

            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.wordFilters.excludeColorWords = true;
            builderParams.wordFilters.requireDrawings = true;
            builder = new RandomWordsQuestionBuilder(nPacks, nCorrect, nWrong, parameters:builderParams);

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
