using Antura.Teacher;

namespace Antura.Minigames.ThrowBalls
{
    public enum ThrowBallsVariation
    {
        Letter = MiniGameCode.ThrowBalls_letter,
        LetterForm = MiniGameCode.ThrowBalls_letterform,
        Word = MiniGameCode.ThrowBalls_word,
        LetterInWord = MiniGameCode.ThrowBalls_letterinword
    }

    public class ThrowBallsConfiguration : AbstractGameConfiguration
    {
        public ThrowBallsVariation Variation { get; set; }

        public override void SetMiniGameCode(MiniGameCode code)
        {
            Variation = (ThrowBallsVariation)code;
        }

        // Singleton Pattern
        static ThrowBallsConfiguration instance;
        public static ThrowBallsConfiguration Instance
        {
            get {
                if (instance == null) {
                    instance = new ThrowBallsConfiguration();
                }
                return instance;
            }
        }

        private ThrowBallsConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            Questions = new ThrowBallsQuestionProvider();
            Variation = ThrowBallsVariation.Letter;
            Context = new MinigamesGameContext(MiniGameCode.ThrowBalls_letter, System.DateTime.Now.Ticks.ToString());
            Difficulty = 0.7f;
            TutorialEnabled = true;
        }

        public override IQuestionBuilder SetupBuilder()
        {
            IQuestionBuilder builder = null;

            int nPacks = 10;
            int nWrong = 4;

            var builderParams = new QuestionBuilderParameters();
            switch (Variation) {
                case ThrowBallsVariation.Letter:
                    builderParams.letterFilters.excludeDiacritics = LetterFilters.ExcludeDiacritics.All;
                    builder = new RandomLettersQuestionBuilder(nPacks, 1, nWrong: nWrong, firstCorrectIsQuestion: true, parameters: builderParams);
                    break;
                case ThrowBallsVariation.LetterForm:
                    var letterAlterationFilters = LetterAlterationFilters.FormsOfSingleLetter;
                    builder = new RandomLetterAlterationsQuestionBuilder(nPacks, 1, nWrong: nWrong, firstCorrectIsQuestion: true, letterAlterationFilters:letterAlterationFilters, parameters: builderParams);
                    break;
                case ThrowBallsVariation.Word:
                    builderParams.wordFilters.requireDrawings = true;
                    builder = new RandomWordsQuestionBuilder(nPacks, 1, nWrong, firstCorrectIsQuestion: true, parameters: builderParams);
                    break;
                case ThrowBallsVariation.LetterInWord:
                    builder = new LettersInWordQuestionBuilder(nPacks, maximumWordLength: 7, nWrong: nWrong, useAllCorrectLetters: true, parameters: builderParams);
                    break;
            }

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
