using Antura.Database;
using Antura.Teacher;
using System;

namespace Antura.Minigames.Egg
{
    public enum EggVariation
    {
        LetterName = MiniGameCode.Egg_lettername,
        LetterPhoneme = MiniGameCode.Egg_letterphoneme,
        BuildWord = MiniGameCode.Egg_buildword
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
            get {
                if (instance == null) {
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

            if (Variation == EggVariation.BuildWord) {
                Questions = new SampleEggSequenceQuestionProvider();
            } else {
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

            switch (Variation) {
                case EggVariation.LetterName:
                    builder = new RandomLettersQuestionBuilder(nPacks, nCorrect, nWrong, parameters: builderParams);
                    break;
                case EggVariation.BuildWord:
                    builder = new LettersInWordQuestionBuilder(nPacks, nWrong: nWrong, useAllCorrectLetters: true,
                        parameters: builderParams);
                    break;
                case EggVariation.LetterPhoneme:
                    builder = new RandomLetterAlterationsQuestionBuilder(nPacks, 1, 3, parameters: builderParams, letterAlterationFilters: LetterAlterationFilters.FormsAndPhonemesOfMultipleLetters_OneForm);
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
            get {
                switch (Variation) {
                    case EggVariation.LetterName:
                        return LocalizationDataId.Egg_letters_Title;
                    case EggVariation.LetterPhoneme:
                        return LocalizationDataId.Egg_letterphoneme_Title;
                    case EggVariation.BuildWord:
                        return LocalizationDataId.Egg_buildword_Title;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public bool IsSingleVariation()
        {
            switch (Variation) {
                case EggVariation.LetterName:
                case EggVariation.LetterPhoneme:
                    return true;
                case EggVariation.BuildWord:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool IsSequence()
        {
            switch (Variation) {
                case EggVariation.LetterName:
                case EggVariation.LetterPhoneme:
                    return false;
                case EggVariation.BuildWord:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion

    }
}