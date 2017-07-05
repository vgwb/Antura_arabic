using Antura.MinigamesAPI;
using Antura.MinigamesCommon;
using Antura.Teacher;

namespace Antura.Minigames.Egg
{
    public enum EggVariation
    {
        Single = MiniGameCode.Egg_letters,
        Sequence = MiniGameCode.Egg_sequence
    }

    public class EggConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider Questions { get; set; }

        public float Difficulty { get; set; }
        public bool TutorialEnabled { get; set; }
        public EggVariation Variation { get; set; }

        public void SetMiniGameCode(MiniGameCode code)
        {
            Variation = (EggVariation) code;
        }

        /////////////////
        // Singleton Pattern
        static EggConfiguration instance;
        public static EggConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new EggConfiguration();
                return instance;
            }
        }
        /////////////////

        private EggConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            Context = new MinigamesGameContext(MiniGameCode.Egg_letters, System.DateTime.Now.Ticks.ToString());
            Difficulty = 0.1f;
            Variation = EggVariation.Single;

            if (Variation == EggVariation.Sequence)
                Questions = new SampleEggSequenceQuestionProvider();
            else
                Questions = new SampleEggSingleQuestionProvider();

            TutorialEnabled = true;
        }

        public IQuestionBuilder SetupBuilder()
        {
            IQuestionBuilder builder = null;

            int nPacks = 10;
            int nCorrect = 6;
            int nWrong = 7;

            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.correctSeverity = Teacher.SelectionSeverity.AsManyAsPossible;

            builder = new RandomLettersQuestionBuilder(nPacks, nCorrect, nWrong, parameters: builderParams);

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