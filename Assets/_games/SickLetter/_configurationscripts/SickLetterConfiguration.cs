namespace EA4S.SickLetter {
    public enum SickLetterVariation : int {
        V_1 = 1,
    }

    public class SickLetterConfiguration : IGameConfiguration {
        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider Questions { get; set; }

        #region Game configurations
        public float Difficulty { get; set; }
        public SickLetterVariation Variation { get; set; }
        #endregion

        /////////////////
        // Singleton Pattern
        static SickLetterConfiguration instance;
        public static SickLetterConfiguration Instance {
            get {
                if (instance == null)
                    instance = new SickLetterConfiguration();
                return instance;
            }
        }
        /////////////////

        private SickLetterConfiguration() {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE

            Questions = new SampleQuestionProvider();

            Variation = SickLetterVariation.V_1;

            Context = new SampleGameContext();
            Difficulty = 0.5f;
        }

        #region external configuration call
        public static void SetConfiguration(float _difficulty, int _variation) {
            instance = new SickLetterConfiguration() {
                Difficulty = _difficulty,
                Variation = (SickLetterVariation)_variation,
            };
        }
        #endregion
    }
}
