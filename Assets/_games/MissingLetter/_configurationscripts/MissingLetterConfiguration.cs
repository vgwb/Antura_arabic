namespace EA4S.MissingLetter
{
    public enum MissingLetterVariation : int
    {
        MissingLetter = 1,
        MissingWord = 2
    }

    public class MissingLetterConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider Questions { get; set; }

        public float Difficulty { get; set; }

        #region Game configurations
        public MissingLetterVariation Variation { get; set; }
        #endregion

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

            Difficulty = 0.5f;
            Variation = MissingLetterVariation.MissingLetter;
        }

        public IQuestionBuilder SetupBuilder() {
            IQuestionBuilder builder = null;

            int nPacks = 10;
            int nCorrect = 1;
            int nWrong = 5;

            var builderParams = new Teacher.QuestionBuilderParameters();

            switch (Variation)
            {
                case MissingLetterVariation.MissingLetter:
                    builder = new LettersInWordQuestionBuilder(nPacks, nCorrect, nWrong, parameters: builderParams);
                    break;

                case MissingLetterVariation.MissingWord:
                    builderParams.phraseFilters.requireWords = true;
                    builderParams.phraseFilters.requireAtLeastTwoWords = true;
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
