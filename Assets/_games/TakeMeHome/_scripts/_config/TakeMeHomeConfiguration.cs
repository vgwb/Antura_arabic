using EA4S.MinigamesAPI;
using EA4S.MinigamesAPI.Sample;
using EA4S.MinigamesCommon;
using EA4S.Teacher;

namespace EA4S.Minigames.TakeMeHome
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
			Context = new MinigamesGameContext(MiniGameCode.TakeMeHome, System.DateTime.Now.Ticks.ToString());
			Letters = new TakeMeHomeLettersProvider();
            Questions = new SampleQuestionProvider();
            Difficulty = 0;
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

            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.letterFilters.excludeDiacritics = LetterFilters.ExcludeDiacritics.All;
            builderParams.letterFilters.excludeLetterVariations = LetterFilters.ExcludeLetterVariations.All;
            builderParams.wordFilters.excludeDiacritics = true;
            builderParams.wordFilters.excludeLetterVariations = true;
            builder = new RandomLettersQuestionBuilder(1,7, parameters: builderParams);

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
