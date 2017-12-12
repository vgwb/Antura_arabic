using Antura.LivingLetters.Sample;
using Antura.Teacher;

namespace Antura.Minigames.Tobogan
{
    public enum ToboganVariation
    {
        LetterInWord = MiniGameCode.Tobogan_letterinword,
        SunMoon = MiniGameCode.Tobogan_sunmoon
    }

    public class ToboganConfiguration : AbstractGameConfiguration
    {
        public ToboganVariation Variation { get; set; }

        public override void SetMiniGameCode(MiniGameCode code)
        {
            Variation = (ToboganVariation)code;
        }

        // Singleton Pattern
        static ToboganConfiguration instance;
        public static ToboganConfiguration Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ToboganConfiguration();
                }
                return instance;
            }
        }

        private ToboganConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            Questions = new SampleQuestionProvider();
            //Questions = new SunMoonQuestionProvider();

            //Variation = ToboganVariation.SunMoon;
            Variation = ToboganVariation.LetterInWord;

            Context = new MinigamesGameContext(MiniGameCode.Tobogan_letterinword, System.DateTime.Now.Ticks.ToString());
            Difficulty = 0.0f;
            TutorialEnabled = true;
        }

        public override IQuestionBuilder SetupBuilder()
        {
            IQuestionBuilder builder = null;

            int nPacks = 10;
            int nCorrect = 1;
            int nWrong = 5;

            var builderParams = new QuestionBuilderParameters();
            switch (Variation)
            {
                case ToboganVariation.LetterInWord:
                    builderParams.wordFilters.excludeLetterVariations = true;
                    builder = new LettersInWordQuestionBuilder(nPacks, nCorrect: nCorrect, nWrong: nWrong, parameters: builderParams);
                    break;
                case ToboganVariation.SunMoon:
                    builder = new WordsBySunMoonQuestionBuilder(nPacks, parameters: builderParams);
                    break;
            }

            if (builder == null)
            {
                throw new System.Exception("No question builder defined for variation " + Variation.ToString());
            }
            return builder;
        }

        public override MiniGameLearnRules SetupLearnRules()
        {
            var rules = new MiniGameLearnRules();
            // example: a.minigameVoteSkewOffset = 1f;
            return rules;
        }

        public int GetDiscreteDifficulty(int maximum)
        {
            int d = (int)Difficulty * (maximum + 1);

            if (d > maximum)
                return maximum;
            return d;
        }

    }
}
