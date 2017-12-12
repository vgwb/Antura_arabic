using Antura.LivingLetters;
using Antura.Teacher;

namespace Antura.Minigames.ColorTickle
{
    public enum ColorTickleVariation
    {
        Default = MiniGameCode.ColorTickle_letter,
    }

    public class ColorTickleConfiguration : AbstractGameConfiguration
    {
        public ColorTickleVariation Variation { get; set; }

        public override void SetMiniGameCode(MiniGameCode code)
        {
            Variation = (ColorTickleVariation)code;
        }

        // Singleton Pattern
        static ColorTickleConfiguration instance;
        public static ColorTickleConfiguration Instance
        {
            get {
                if (instance == null) {
                    instance = new ColorTickleConfiguration();
                }
                return instance;
            }
        }

        private ColorTickleConfiguration()
        {
            // Default values
            Questions = new ColorTickleLetterProvider();
            Context = new MinigamesGameContext(MiniGameCode.ColorTickle_letter, System.DateTime.Now.Ticks.ToString());
            Difficulty = 0.5f;
            TutorialEnabled = true;
            Variation = ColorTickleVariation.Default;
        }

        public override IQuestionBuilder SetupBuilder()
        {
            IQuestionBuilder builder = null;

            int nPacks = 10;
            int nCorrect = 1;

            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.letterFilters.excludeDiacritics = LetterFilters.ExcludeDiacritics.All;
            builderParams.letterFilters.excludeLetterVariations = LetterFilters.ExcludeLetterVariations.AllButAlefHamza;
            builderParams.letterFilters.excludeDiphthongs = true;
            builderParams.wordFilters.excludeDiacritics = true;
            builder = new RandomLettersQuestionBuilder(nPacks, nCorrect, parameters: builderParams);

            return builder;
        }

        public override MiniGameLearnRules SetupLearnRules()
        {
            var rules = new MiniGameLearnRules();
            // example: a.minigameVoteSkewOffset = 1f;
            return rules;
        }
    }
}
