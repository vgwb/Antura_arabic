using Antura.MinigamesAPI;
using Antura.MinigamesAPI.Sample;
using Antura.MinigamesCommon;
using Antura.Teacher;

namespace Antura.Minigames.MixedLetters
{
    public class MixedLettersConfiguration : IGameConfiguration
    {
        public enum MixedLettersVariation : int {
            Alphabet = 1,
            Spelling = 2,
        }

        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider Questions { get; set; }
        public float Difficulty { get; set; }
        public bool TutorialEnabled { get; set; }
        public MixedLettersVariation Variation { get; set; }

        /////////////////
        // Singleton Pattern
        static MixedLettersConfiguration instance;
        public static MixedLettersConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new MixedLettersConfiguration();
                return instance;
            }
        }
        /////////////////

        private MixedLettersConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            Questions = new SampleQuestionProvider();
            Variation = MixedLettersVariation.Alphabet;
            Context = new MinigamesGameContext(MiniGameCode.MixedLetters_alphabet, System.DateTime.Now.Ticks.ToString());
            Difficulty = 0.5f;
            TutorialEnabled = true;
        }

        public IQuestionBuilder SetupBuilder() {
            IQuestionBuilder builder = null;

            int nPacks = 10;

            var builderParams = new Teacher.QuestionBuilderParameters();
            switch (Variation)
            {
                case MixedLettersVariation.Alphabet:
                    builderParams.useJourneyForCorrect = false; // Force no journey, or the minigame will block
                    builder = new AlphabetQuestionBuilder(parameters: builderParams);
                    break;
                case MixedLettersVariation.Spelling:
                    builder = new LettersInWordQuestionBuilder(nPacks, maximumWordLength: 6, useAllCorrectLetters: true, parameters: builderParams);
                    break;
            }

            UnityEngine.Debug.Log("Chosen variation: " + Variation);

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
