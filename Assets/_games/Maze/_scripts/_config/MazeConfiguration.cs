using Antura.LivingLetters;
using Antura.LivingLetters.Sample;
using Antura.Teacher;

namespace Antura.Minigames.Maze
{
    public enum MazeVariation
    {
        Default = MiniGameCode.Maze_letter
    }

    public class MazeConfiguration : AbstractGameConfiguration
    {
        public MazeVariation Variation { get; set; }

        public override void SetMiniGameCode(MiniGameCode code)
        {
            Variation = (MazeVariation)code;
        }

        // Singleton Pattern
        static MazeConfiguration instance;
        public static MazeConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new MazeConfiguration();
                return instance;
            }
        }

        private MazeConfiguration()
        {
            // Default values
            Questions = new SampleQuestionProvider();
            Variation = MazeVariation.Default;

            Context = new MinigamesGameContext(MiniGameCode.Maze_letter, System.DateTime.Now.Ticks.ToString());
            Difficulty = 0.5f;
            TutorialEnabled = false;
        }

        public override IQuestionBuilder SetupBuilder()
        {
            IQuestionBuilder builder = null;

            var builderParams = new QuestionBuilderParameters();
            builderParams.letterFilters.excludeDiacritics = LetterFilters.ExcludeDiacritics.All;
            builderParams.wordFilters.excludeDiacritics = true;
            builderParams.letterFilters.excludeLetterVariations = LetterFilters.ExcludeLetterVariations.AllButAlefHamza;
            builder = new RandomLettersQuestionBuilder(7, 1, parameters: builderParams);

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
