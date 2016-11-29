namespace EA4S.MissingLetter
{
    //enum game type
    public enum MissingLetterVariation : int
    {
        WORD = 1,
        PHRASE = 2
    }

    public class MissingLetterConfiguration : IGameConfiguration
    {


        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider Questions { get; set; }

        public float Difficulty { get; set; }
        public MissingLetterVariation Variation { get; set; }


        /////////////////
        // Singleton Pattern
        static MissingLetterConfiguration instance;
        public static MissingLetterConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new MissingLetterConfiguration();
                return instance;
            }
        }
        /////////////////

        private MissingLetterConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            Questions = new SampleQuestionProvider();
            Context = new SampleGameContext();

            Variation = MissingLetterVariation.WORD;
            Difficulty = 0.5f;
        }

        public static void SetConfiguration(float _difficulty, int _variation)
        {
            instance = new MissingLetterConfiguration()
            {
                Difficulty = _difficulty,
                Variation = (MissingLetterVariation)_variation,
            };
        }

        public IQuestionBuilder SetupBuilder() {
            IQuestionBuilder builder = null;

            int nPacks = 10;
            int nCorrect = 1;
            int nWrong = 5;

            var builderParams = new Teacher.QuestionBuilderParameters();
            switch (Variation)
            {
                case MissingLetterVariation.WORD:
                    builder = new LettersInWordQuestionBuilder(nPacks, nCorrect, nWrong, parameters: builderParams);
                    break;
                case MissingLetterVariation.PHRASE:
                    builder = new WordsInPhraseQuestionBuilder(nPacks, nCorrect, nWrong, parameters: builderParams);
                    break;
            }

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
