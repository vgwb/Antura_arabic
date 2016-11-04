namespace EA4S.Scanner 
{

	public enum ScannerVariation : int 
	{
        V_1 = 1,
        phrase = 2,
    }

    public class ScannerConfiguration : IGameConfiguration 
	{
        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider Questions { get; set; }

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

        private ScannerConfiguration() {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE

            Questions = new SampleQuestionProvider();

            Variation = ScannerVariation.V_1;

            Context = new SampleGameContext();
            Difficulty = 0.5f;
        }

        #region external configuration call
        public static void SetConfiguration(float _difficulty, int _variation) {
            instance = new ScannerConfiguration() {
                Difficulty = _difficulty,
                Variation = (ScannerVariation)_variation,
            };
        }
        #endregion
    }
}
