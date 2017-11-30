using Antura.LivingLetters;
using Antura.Teacher;

namespace Antura.Minigames.ReadingGame
{
    public enum ReadingGameVariation
    {
        ReadAndAnswer = MiniGameCode.ReadingGame_word,
        AlphabetSong = MiniGameCode.AlphabetSong_alphabet,
        DiacriticSong = MiniGameCode.AlphabetSong_letter,
    }

    public class ReadingGameConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider Questions { get; set; }
        public ReadingGameVariation Variation { get; set; }

        public float Difficulty { get; set; }
        public bool TutorialEnabled { get; set; }

        public void SetMiniGameCode(MiniGameCode code)
        {
            Variation = (ReadingGameVariation)code;
        }

        public int GetDiscreteDifficulty(int maximum)
        {
            int d = (int)Difficulty * (maximum + 1);

            if (d > maximum)
            {
                return maximum;
            }
            return d;
        }

        /////////////////
        // Singleton Pattern
        static ReadingGameConfiguration instance;
        public static ReadingGameConfiguration Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ReadingGameConfiguration();
                }
                return instance;
            }
        }
        /////////////////

        private ReadingGameConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            Questions = new SampleReadingGameQuestionProvider();
            Variation = ReadingGameVariation.ReadAndAnswer;
            //Variation = ReadingGameVariation.AlphabetSong;

            Context = new MinigamesGameContext(MiniGameCode.ReadingGame_word, System.DateTime.Now.Ticks.ToString());
            Difficulty = 0.0f;
            TutorialEnabled = true;
        }

        public IQuestionBuilder SetupBuilder()
        {
            IQuestionBuilder builder = null;

            var builderParams = new Teacher.QuestionBuilderParameters();
            switch (Variation)
            {
                case ReadingGameVariation.AlphabetSong:
                case ReadingGameVariation.DiacriticSong:
                    builder = new EmptyQuestionBuilder();
                    break;
                case ReadingGameVariation.ReadAndAnswer:
                    builderParams.wordFilters.excludeColorWords = true;
                    builderParams.wordFilters.requireDrawings = true;
                    builderParams.phraseFilters.requireAnswersOrWords = true;
                    builder = new WordsInPhraseQuestionBuilder(nPacks: 10, nCorrect: 1, nWrong: 6, usePhraseAnswersIfFound: true, parameters: builderParams);
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
