using Antura.MinigamesAPI;
using Antura.MinigamesCommon;
using Antura.Teacher;

namespace Antura.Minigames.DancingDots {

    public class DancingDotsConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider Questions { get; set; }

        #region Game configurations
        public float Difficulty { get; set; }
        public bool TutorialEnabled { get; set; }
        public DancingDotsVariation Variation { get; set; }
        #endregion

        /////////////////
        // Singleton Pattern
        static DancingDotsConfiguration instance;
        public static DancingDotsConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new DancingDotsConfiguration();
                return instance;
            }
        }
        /////////////////

        private DancingDotsConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
			Context = new MinigamesGameContext(MiniGameCode.DancingDots, System.DateTime.Now.Ticks.ToString());

            Variation = DancingDotsVariation.V_1;
			Questions = new DancingDotsQuestionProvider();
            TutorialEnabled = true;
        }

        #region external configuration call
        public static void SetConfiguration(float _difficulty, int _variation)
        {
            instance = new DancingDotsConfiguration()
            {
                Difficulty = _difficulty,
                Variation = (DancingDotsVariation)_variation,
            };
        }
        #endregion

        public IQuestionBuilder SetupBuilder()
        {
            IQuestionBuilder builder = null;

            int nPacks = TutorialEnabled ? 7 : 6; // extra one for the tutorial
            int nCorrect = 1;
            int nWrong = 0;

            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.letterFilters.excludeDiacritics = LetterFilters.ExcludeDiacritics.AllButMain;
            builderParams.letterFilters.excludeLetterVariations = LetterFilters.ExcludeLetterVariations.All;
            builderParams.wordFilters.excludeDiacritics = false;
            builderParams.wordFilters.excludeLetterVariations = true;
            builderParams.letterFilters.excludeDiphthongs = true;
            builder = new RandomLettersQuestionBuilder(nPacks, nCorrect, nWrong, parameters:builderParams);

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
