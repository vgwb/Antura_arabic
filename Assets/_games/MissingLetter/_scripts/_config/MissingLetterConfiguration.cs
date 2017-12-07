using Antura.LivingLetters;
using Antura.LivingLetters.Sample;
using Antura.Teacher;

namespace Antura.Minigames.MissingLetter
{
    public enum MissingLetterVariation
    {
        Phrase = MiniGameCode.MissingLetter_phrase,
        LetterForm = MiniGameCode.MissingLetter_letterform,
        LetterInWord = MiniGameCode.MissingLetter_letterinword
    }

    public class MissingLetterConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider Questions { get; set; }

        public float Difficulty { get; set; }
        public bool TutorialEnabled { get; set; }
        public MissingLetterVariation Variation { get; set; }

        public void SetMiniGameCode(MiniGameCode code)
        {
            Variation = (MissingLetterVariation)code;
        }

        /////////////////
        // Singleton Pattern
        static MissingLetterConfiguration instance;
        public static MissingLetterConfiguration Instance
        {
            get {
                if (instance == null) {
                    instance = new MissingLetterConfiguration();
                }
                return instance;
            }
        }
        /////////////////

        private MissingLetterConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            Questions = new SampleQuestionProvider();
            Context = new MinigamesGameContext(MiniGameCode.MissingLetter_letter, System.DateTime.Now.Ticks.ToString());

            Difficulty = 0.5f;
            //Variation = MissingLetterVariation.MissingLetter;
            Variation = MissingLetterVariation.LetterInWord;
            TutorialEnabled = true;
        }

        public IQuestionBuilder SetupBuilder()
        {
            IQuestionBuilder builder = null;

            int nPacks = 10;
            int nCorrect = 1;
            int nWrong = 5;

            var builderParams = new QuestionBuilderParameters();

            switch (Variation) {
                case MissingLetterVariation.LetterInWord:
                    builderParams.letterFilters.excludeDiacritics = LetterFilters.ExcludeDiacritics.All;
                    builderParams.letterFilters.excludeDiphthongs = true;
                    builder = new LettersInWordQuestionBuilder(nPacks, nCorrect: nCorrect, nWrong: nWrong, forceUnseparatedLetters: true, parameters: builderParams);
                    break;

                case MissingLetterVariation.LetterForm:
                    builder = new LetterFormsInWordsQuestionBuilder(1, 1, parameters: builderParams);
                    break;

                case MissingLetterVariation.Phrase:
                    builderParams.phraseFilters.requireWords = true;
                    builderParams.phraseFilters.requireAtLeastTwoWords = true;
                    builder = new WordsInPhraseQuestionBuilder(nPacks, nCorrect, nWrong, parameters: builderParams);
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
