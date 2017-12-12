using System;
using Antura.Database;
using Antura.Teacher;

namespace Antura.Minigames.Egg
{
    public enum EggVariation
    {
        LetterName = MiniGameCode.Egg_lettername,
        LetterPhoneme = MiniGameCode.Egg_letterphoneme,
        Sequence = MiniGameCode.Egg_sequence
    }

    public class EggConfiguration : AbstractGameConfiguration
    {
        private EggVariation Variation { get; set; }

        public override void SetMiniGameCode(MiniGameCode code)
        {
            Variation = (EggVariation)code;
        }

        // Singleton Pattern
        static EggConfiguration instance;
        public static EggConfiguration Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EggConfiguration();
                }
                return instance;
            }
        }

        private EggConfiguration()
        {
            // Default values
            Context = new MinigamesGameContext(MiniGameCode.Egg_lettername, System.DateTime.Now.Ticks.ToString());
            Difficulty = 0.1f;
            Variation = EggVariation.LetterName;

            if (Variation == EggVariation.Sequence)
            {
                Questions = new SampleEggSequenceQuestionProvider();
            }
            else
            {
                Questions = new SampleEggSingleQuestionProvider();
            }
            TutorialEnabled = true;
        }

        public override IQuestionBuilder SetupBuilder()
        {
            IQuestionBuilder builder = null;

            int nPacks = 10;
            int nCorrect = 6;
            int nWrong = 7;

            var builderParams = new QuestionBuilderParameters();
            builderParams.correctSeverity = SelectionSeverity.AsManyAsPossible;

            switch (Variation)
            {
                case EggVariation.LetterName:
                    builder = new RandomLettersQuestionBuilder(nPacks, nCorrect, nWrong, parameters: builderParams);
                    break;
                case EggVariation.Sequence:
                    builder = new RandomLettersQuestionBuilder(nPacks, nCorrect, nWrong, parameters: builderParams);
                    break;
                case EggVariation.LetterPhoneme:
                    builder = new RandomLetterAlterationsQuestionBuilder(nPacks, 1, 3, parameters: builderParams, letterAlterationFilters: LetterAlterationFilters.PhonemesOfMultipleLetters);
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

        #region Variation checks

        public override LocalizationDataId TitleLocalizationId
        {
            get
            {
                switch (Variation)
                {
                    case EggVariation.LetterName:
                    case EggVariation.LetterPhoneme:
                        return LocalizationDataId.Egg_letters_Title;
                    case EggVariation.Sequence:
                        return LocalizationDataId.Egg_sequence_Title;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public bool IsSingleVariation()
        {
            switch (Variation)
            {
                case EggVariation.LetterName:
                case EggVariation.LetterPhoneme:
                    return true;
                case EggVariation.Sequence:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool IsSequence()
        {
            switch (Variation)
            {
                case EggVariation.LetterName:
                case EggVariation.LetterPhoneme:
                    return false;
                case EggVariation.Sequence:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion

    }
}