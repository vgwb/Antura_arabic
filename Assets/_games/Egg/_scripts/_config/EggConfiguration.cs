using System;
using Antura.Database;
using Antura.LivingLetters;
using Antura.Teacher;

namespace Antura.Minigames.Egg
{
    public enum EggVariation
    {
        Letter = MiniGameCode.Egg_letter,
        LetterForm = MiniGameCode.Egg_letterform,
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
            Variation = (EggVariation)code;
        }

        /////////////////
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

        /////////////////

        private EggConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            Context = new MinigamesGameContext(MiniGameCode.Egg_letter, System.DateTime.Now.Ticks.ToString());
            Difficulty = 0.1f;
            Variation = EggVariation.Letter;

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

        public IQuestionBuilder SetupBuilder()
        {
            IQuestionBuilder builder = null;

            int nPacks = 10;
            int nCorrect = 6;
            int nWrong = 7;

            var builderParams = new QuestionBuilderParameters();
            builderParams.correctSeverity = SelectionSeverity.AsManyAsPossible;

            switch (Variation)
            {
                case EggVariation.Letter:
                    builder = new RandomLettersQuestionBuilder(nPacks, nCorrect, nWrong, parameters: builderParams);
                    break;
                case EggVariation.Sequence:
                    builder = new RandomLettersQuestionBuilder(nPacks, nCorrect, nWrong, parameters: builderParams);
                    break;
                case EggVariation.LetterForm:
                    builder = new RandomLetterAlterationsQuestionBuilder(nPacks, 1, 3, parameters: builderParams, letterAlterationFilters: LetterAlterationFilters.PhonemesOfMultipleLetters);
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

        #region Variation checks

        public LocalizationDataId TitleLocalizationId
        {
            get
            {
                switch (Variation)
                {
                    case EggVariation.Letter:
                    case EggVariation.LetterForm:
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
                case EggVariation.Letter:
                case EggVariation.LetterForm:
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
                case EggVariation.Letter:
                case EggVariation.LetterForm:
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