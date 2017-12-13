using System;
using Antura.LivingLetters;
using Antura.LivingLetters.Sample;
using Antura.Teacher;

namespace Antura.Minigames.MixedLetters
{
    public enum MixedLettersVariation
    {
        Alphabet = MiniGameCode.MixedLetters_alphabet,
        BuildWord = MiniGameCode.MixedLetters_buildword
    }

    public class MixedLettersConfiguration : AbstractGameConfiguration
    {
        public MixedLettersVariation Variation { get; private set; }

        public override void SetMiniGameCode(MiniGameCode code)
        {
            Variation = (MixedLettersVariation)code;
        }

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

        private MixedLettersConfiguration()
        {
            // Default values
            Questions = new SampleQuestionProvider();
            Variation = MixedLettersVariation.Alphabet;
            Context = new MinigamesGameContext(MiniGameCode.MixedLetters_alphabet, DateTime.Now.Ticks.ToString());
            Difficulty = 0.5f;
            TutorialEnabled = true;
        }

        public override IQuestionBuilder SetupBuilder()
        {
            IQuestionBuilder builder = null;

            int nPacks = 10;

            var builderParams = new QuestionBuilderParameters();
            switch (Variation)
            {
                case MixedLettersVariation.Alphabet:
                    builderParams.useJourneyForCorrect = false; // Force no journey, or the minigame will block
                    builder = new AlphabetQuestionBuilder(parameters: builderParams);
                    break;
                case MixedLettersVariation.BuildWord:
                    builder = new LettersInWordQuestionBuilder(nPacks, maximumWordLength: 6, useAllCorrectLetters: true, parameters: builderParams);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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
