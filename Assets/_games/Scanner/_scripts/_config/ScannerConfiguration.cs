using EA4S.MinigamesAPI;
using EA4S.MinigamesAPI.Sample;
using EA4S.MinigamesCommon;
using EA4S.Teacher;

namespace EA4S.Minigames.Scanner 
{

    public class ScannerConfiguration : IGameConfiguration 
	{
        // Game configuration
		public IGameContext Context { get; set; }
		public IQuestionProvider Questions { get; set; }
		

        #region Game configurations
        public float Difficulty { get; set; }
	    public bool TutorialEnabled { get; set; }
	    public ScannerVariation Variation { get; set; }
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

        private ScannerConfiguration() 
		{
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE

			Difficulty = 0.13f;
			Variation = ScannerVariation.OneWord;

            Questions = new SampleQuestionProvider();
            Context = new MinigamesGameContext(MiniGameCode.Scanner, System.DateTime.Now.Ticks.ToString());
		    TutorialEnabled = true;
		}

        #region external configuration call
        public static void SetConfiguration(float _difficulty, int _variation) {
            instance = new ScannerConfiguration() {
                Difficulty = _difficulty,
				Variation = (ScannerVariation) _variation
            };
        }
        #endregion

		

		public int nCorrect = 1;

        public IQuestionBuilder SetupBuilder() {

            IQuestionBuilder builder = null;


			int nPacks = 7; // One Extra for tutorial
			nCorrect = 1;
			int nWrong = 4;

            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.wordFilters.excludeColorWords = true;
            builderParams.wordFilters.requireDrawings = true;

			switch (Variation)
			{
			case ScannerVariation.OneWord:
				nCorrect = 1;
				nWrong = 4;
				break;
			case ScannerVariation.MultipleWords:
				if (Difficulty < 0.5f)
				{
					nCorrect = 3;
				}
				else
				{
					nCorrect = 5;
				}
				nWrong = 0;
				break;
			}

			builder = new RandomWordsQuestionBuilder(nPacks, nCorrect, nWrong, parameters:builderParams);


			if (builder == null)
				throw new System.Exception("No question builder defined for variation " + Variation.ToString());


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
