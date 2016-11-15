namespace EA4S.TakeMeHome
{
	
	public class TakeMeHomeConfiguration : IGameConfiguration
	{
		// Game configuration
		public IGameContext Context { get; set; }
        public IQuestionProvider Questions { get; set; }

        public ILivingLetterDataProvider Letters { get; set; }
		#region Game configurations
		public float Difficulty { get; set; }
		#endregion

		/////////////////
		// Singleton Pattern
		static TakeMeHomeConfiguration instance;
		public static TakeMeHomeConfiguration Instance
		{
			get
			{
				if (instance == null)
					instance = new TakeMeHomeConfiguration();
				return instance;
			}
		}
		/////////////////

		private TakeMeHomeConfiguration()
		{
			// Default values
			Context = new SampleGameContext();
			Letters = new SampleLetterProvider ();

            Difficulty = 0.5f;
		}

		#region external configuration call
		public static void SetConfiguration(float _difficulty, int _variation) {
			instance = new TakeMeHomeConfiguration() {
				Difficulty = _difficulty
			};
		}
        #endregion

        public IQuestionBuilder SetupBuilder() {
            IQuestionBuilder builder = null;

            int nPacks = 10;
            int nCorrect = 5;

            builder = new RandomLettersQuestionBuilder(nPacks, nCorrect);

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
