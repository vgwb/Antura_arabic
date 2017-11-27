using Antura.LivingLetters;
using Antura.LivingLetters.Sample;
using Antura.Minigames;
using Antura.Teacher;

namespace Antura.Minigames.Maze
{
    public enum MazeVariation
    {
        Default = MiniGameCode.Maze
    }

    public class MazeConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider Questions { get; set; }
        public ILivingLetterDataProvider Letters { get; set; }

        #region Game configurations

        public float Difficulty { get; set; }
        public bool TutorialEnabled { get; set; }
        public MazeVariation Variation { get; set; }

        public void SetMiniGameCode(MiniGameCode code)
        {
            Variation = (MazeVariation)code;
        }

        #endregion

        /////////////////
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
        /////////////////

        private MazeConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE

            Questions = new SampleQuestionProvider();
            Letters = new MazeLetterProvider();
            Variation = MazeVariation.Default;

            Context = new MinigamesGameContext(MiniGameCode.Maze, System.DateTime.Now.Ticks.ToString());
            Difficulty = 0.5f;
            TutorialEnabled = false;
        }

        #region external configuration call
        public static void SetConfiguration(float _difficulty, int _variation)
        {
            instance = new MazeConfiguration()
            {
                Difficulty = _difficulty,
                Variation = (MazeVariation)_variation,
            };
        }
        #endregion

        public IQuestionBuilder SetupBuilder()
        {
            IQuestionBuilder builder = null;

            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.letterFilters.excludeDiacritics = LetterFilters.ExcludeDiacritics.All;
            builderParams.wordFilters.excludeDiacritics = true;
            builderParams.letterFilters.excludeLetterVariations = LetterFilters.ExcludeLetterVariations.AllButAlefHamza;
            builder = new RandomLettersQuestionBuilder(7, 1, parameters: builderParams);

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
