using Antura.LivingLetters;
using Antura.Minigames;
using Antura.Teacher;

namespace Antura.Minigames.ThrowBalls
{
    public enum ThrowBallsVariation
    {
        Letters = MiniGameCode.ThrowBalls_letters,
        Words = MiniGameCode.ThrowBalls_words,
        LettersInWord = MiniGameCode.ThrowBalls_letterinword
    }

    public class ThrowBallsConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider Questions { get; set; }

        #region Game configurations

        public float Difficulty { get; set; }
        public bool TutorialEnabled { get; set; }
        public ThrowBallsVariation Variation { get; set; }

        public void SetMiniGameCode(MiniGameCode code)
        {
            Variation = (ThrowBallsVariation)code;
        }

        #endregion

        /////////////////
        // Singleton Pattern
        static ThrowBallsConfiguration instance;
        public static ThrowBallsConfiguration Instance {
            get {
                if (instance == null)
                    instance = new ThrowBallsConfiguration();
                return instance;
            }
        }
        /////////////////

        private ThrowBallsConfiguration() {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            Questions = new ThrowBallsQuestionProvider();
            Variation = ThrowBallsVariation.Letters;
            Context = new MinigamesGameContext(MiniGameCode.ThrowBalls_letters, System.DateTime.Now.Ticks.ToString());
            Difficulty = 0.7f;
            TutorialEnabled = true;
        }

        #region external configuration call
        public static void SetConfiguration(float _difficulty, int _variation) {
            instance = new ThrowBallsConfiguration() {
                Difficulty = _difficulty,
                Variation = (ThrowBallsVariation)_variation,
            };
        }
        #endregion

        public IQuestionBuilder SetupBuilder() {
            IQuestionBuilder builder = null;

            int nPacks = 10;
            int nWrong = 4;

            var builderParams = new Teacher.QuestionBuilderParameters();
            switch (Variation)
            {
                case ThrowBallsVariation.Letters:
                    builder = new RandomLettersQuestionBuilder(nPacks, 1, nWrong: nWrong, firstCorrectIsQuestion: true, parameters: builderParams);
                    break;
                case ThrowBallsVariation.Words:
                    builderParams.wordFilters.requireDrawings = true;
                    builder = new RandomWordsQuestionBuilder(nPacks, 1, nWrong, firstCorrectIsQuestion:true, parameters: builderParams);
                    break;
                case ThrowBallsVariation.LettersInWord:
                    builder = new LettersInWordQuestionBuilder(nPacks, maximumWordLength:7, nWrong:nWrong, useAllCorrectLetters:true, parameters: builderParams);
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
