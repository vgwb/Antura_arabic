using EA4S.MinigamesAPI;
using EA4S.MinigamesAPI.Sample;
using EA4S.MinigamesCommon;
using EA4S.Teacher;

namespace EA4S.Minigames.Tobogan
{
    public enum ToboganVariation : int
    {
        LetterInAWord = 1,
        SunMoon = 2
    }

    public class ToboganConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider Questions { get; set; }

        public float Difficulty { get; set; }
        public bool PerformTutorial { get; set; }
        public ToboganVariation Variation { get; set; }

        public int GetDiscreteDifficulty(int maximum)
        {
            int d = (int) Difficulty * (maximum + 1);

            if (d > maximum)
                return maximum;
            return d;
        }

        /////////////////
        // Singleton Pattern
        static ToboganConfiguration instance;
        public static ToboganConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new ToboganConfiguration();
                return instance;
            }
        }
        /////////////////

        private ToboganConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            Questions = new SampleQuestionProvider();
            //Questions = new SunMoonQuestionProvider();
            
            //Variation = ToboganVariation.SunMoon;
            Variation = ToboganVariation.LetterInAWord;

            Context = new MinigamesGameContext(MiniGameCode.Tobogan_letters, System.DateTime.Now.Ticks.ToString());
            Difficulty = 0.0f;
            PerformTutorial = true;
        }

        public IQuestionBuilder SetupBuilder() {
            IQuestionBuilder builder = null;

            int nPacks = 10;
            int nCorrect = 1;
            int nWrong = 5;

            var builderParams = new Teacher.QuestionBuilderParameters();
            switch (Variation)
            {
                case ToboganVariation.LetterInAWord:
                    builderParams.wordFilters.excludeLetterVariations = true;
                    builder = new LettersInWordQuestionBuilder(nPacks, nCorrect: nCorrect, nWrong: nWrong, parameters: builderParams);
                    break;
                case ToboganVariation.SunMoon:
                    builder = new WordsBySunMoonQuestionBuilder(nPacks, parameters: builderParams);
                    break;
            }

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
