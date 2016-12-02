namespace EA4S.MixedLetters
{
    public class MixedLettersConfiguration : IGameConfiguration
    {
        public enum MixedLettersVariation : int {
            Alphabet = 1,
            Spelling = 2,
        }

        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider Questions { get; set; }
        public float Difficulty { get; set; }
        public MixedLettersVariation Variation { get; set; }

        /////////////////
        // Singleton Pattern
        static MixedLettersConfiguration instance;
        public static MixedLettersConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new MixedLettersConfiguration();
                return instance;
            }
        }
        /////////////////

        private MixedLettersConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            Questions = new SampleQuestionProvider();
            Context = new SampleGameContext();
            Difficulty = 0.5f;
        }

        public IQuestionBuilder SetupBuilder() {
            IQuestionBuilder builder = null;

            int nPacks = 10;

            var builderParams = new Teacher.QuestionBuilderParameters();
            switch (Variation)
            {
                case MixedLettersVariation.Alphabet:
                    builder = new AlphabetQuestionBuilder(parameters: builderParams);
                    break;
                case MixedLettersVariation.Spelling:
                    builder = new LettersInWordQuestionBuilder(nPacks, maximumWordLength: 6, useAllCorrectLetters: true, parameters: builderParams);
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
