using EA4S.MinigamesAPI;
using EA4S.MinigamesCommon;
using EA4S.Teacher;

namespace EA4S.Template
{
    /// <summary>
    /// Template configuration for a minigame.
    /// Use this as a starting point.
    /// </summary>
    public class TemplateConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider Questions { get; set; }
        public float Difficulty { get; set; }
        public bool PerformTutorial { get; set; }

        /////////////////
        // Singleton Pattern
        static TemplateConfiguration instance;
        public static TemplateConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new TemplateConfiguration();
                return instance;
            }
        }
        /////////////////

        private TemplateConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            Context = new MinigamesGameContext(MiniGameCode.Invalid, System.DateTime.Now.Ticks.ToString());
            Difficulty = 0.5f;
        }

        public IQuestionBuilder SetupBuilder() {
            IQuestionBuilder builder = null;
            // CONFIGURE HERE WHAT BUILDER THE MINIGAME IS EXPECTING
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
