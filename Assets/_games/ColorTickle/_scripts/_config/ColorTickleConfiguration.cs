using Antura.LivingLetters;
using Antura.Teacher;

namespace Antura.Minigames.ColorTickle
{
    public class ColorTickleConfiguration : IGameConfiguration
    {
        public enum ColorTickleVariation
        {
            Default = MiniGameCode.ColorTickle_letter,
        }

        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider Questions { get; set; }

        public float Difficulty { get; set; }
        public bool TutorialEnabled { get; set; }
        public ColorTickleVariation Variation { get; set; }

        public void SetMiniGameCode(MiniGameCode code)
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

        public IQuestionBuilder SetupBuilder()
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

        public MiniGameLearnRules SetupLearnRules()
        {
            var rules = new MiniGameLearnRules();
            // example: a.minigameVoteSkewOffset = 1f;
            return rules;
        }
    }
}
