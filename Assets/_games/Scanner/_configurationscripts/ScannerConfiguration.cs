namespace EA4S.Scanner 
{
	public enum ScannerVariation : int
	{
		OneWord = 1,
		MultipleWords = 2
	}

    public class ScannerConfiguration : IGameConfiguration 
	{
        // Game configuration
		public IGameContext Context { get; set; }
		public IQuestionProvider Questions { get; set; }
		public bool gameActive = true;
		public float beltSpeed = 1f;
		public bool facingCamera = true;

        #region Game configurations
        public float Difficulty { get; set; }
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
            Context = new SampleGameContext();
        }

        #region external configuration call
        public static void SetConfiguration(float _difficulty, int _variation) {
            instance = new ScannerConfiguration() {
                Difficulty = _difficulty,
				Variation = (ScannerVariation) _variation
            };
        }
        #endregion

		void SetupVariables()
		{
			gameActive = true;
			beltSpeed = 1f;
			facingCamera = true;

			Difficulty = Difficulty < 0.13f ? 0.13f : Difficulty;

			if (Difficulty <= 0.4f)
			{
				beltSpeed = 1f;
			}
			else if (Difficulty > 0.4f && Difficulty <= 0.6f)
			{
				beltSpeed = 2f;
			}
			else if (Difficulty > 0.6f && Difficulty <= 0.8f)
			{
				beltSpeed = 3f;
			}
			else if (Difficulty > 0.8f && Difficulty < 1f)
			{
				beltSpeed = 4f;
			}
			else if (Difficulty == 1f)
			{
				beltSpeed = 5f;
			}

			if (Difficulty <= 0.25f)
			{
				facingCamera = true;
			}
			else if (Difficulty > 0.25f && Difficulty <= 0.5f)
			{
				facingCamera = true;
			}
			else if (Difficulty > 0.5f && Difficulty <= 0.75f)
			{
				facingCamera = true;
			}
			else if (Difficulty > 0.75f && Difficulty < 1f)
			{
				facingCamera = false;
			}
			else if (Difficulty == 1f)
			{
				facingCamera = false;
			}
		}

		public int nCorrect = 1;

        public IQuestionBuilder SetupBuilder() {

            IQuestionBuilder builder = null;
			SetupVariables();

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
