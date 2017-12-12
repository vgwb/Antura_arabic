using System;
using Antura.Database;
using Antura.LivingLetters;
using Antura.LivingLetters.Sample;
using Antura.Teacher;

namespace Antura.Minigames.FastCrowd
{
    public enum FastCrowdVariation
    {
        BuildWord = MiniGameCode.FastCrowd_buildword,
        Word = MiniGameCode.FastCrowd_word,
        LetterName = MiniGameCode.FastCrowd_lettername,
        LetterForm = MiniGameCode.FastCrowd_letterform,
        Counting = MiniGameCode.FastCrowd_counting,
        Alphabet = MiniGameCode.FastCrowd_alphabet
    }

    public class FastCrowdConfiguration : AbstractGameConfiguration
    {
        public FastCrowdVariation Variation { get; set; }

        public override void SetMiniGameCode(MiniGameCode code)
        {
            Variation = (FastCrowdVariation)code;
        }

        // Singleton Pattern
        static FastCrowdConfiguration instance;
        public static FastCrowdConfiguration Instance
        {
            get {
                if (instance == null) {
                    instance = new FastCrowdConfiguration();
                }
                return instance;
            }
        }

        private FastCrowdConfiguration()
        {
            // Default values
            Questions = new SampleQuestionProvider();
            //Variation = FastCrowdVariation.Letter;
            //Variation = FastCrowdVariation.Alphabet;
            Variation = FastCrowdVariation.BuildWord;

            //Questions = new SampleQuestionWithWordsProvider();
            //Variation = FastCrowdVariation.Counting;

            //Questions = new SampleQuestionWordsVariationProvider();
            //Variation = FastCrowdVariation.Words;
            TutorialEnabled = true;

            Context = new MinigamesGameContext(MiniGameCode.FastCrowd_buildword, System.DateTime.Now.Ticks.ToString());
            Difficulty = 0.5f;
        }

        public override IQuestionBuilder SetupBuilder()
        {
            IQuestionBuilder builder = null;

            int nPacks = 10;
            int nCorrect = 4;
            int nWrong = 4;

            var builderParams = new QuestionBuilderParameters();

            switch (Variation) {
                case FastCrowdVariation.Alphabet:
                    builder = new AlphabetQuestionBuilder();
                    break;
                case FastCrowdVariation.Counting:
                    builder = new OrderedWordsQuestionBuilder(Database.WordDataCategory.Number, builderParams, true);
                    break;
                case FastCrowdVariation.LetterName:
                    // TODO: this is currently a LetterForm, should not!
                    builder = new RandomLettersQuestionBuilder(nPacks, 1, nWrong, firstCorrectIsQuestion: true);
                    break;
                case FastCrowdVariation.LetterForm:
                    // @note: we pass 4 as nCorrect, so we get all the four forms of a single letter, which will be shown one after the other
                    var letterAlterationFilters = LetterAlterationFilters.FormsOfSingleLetter;
                    builder = new RandomLetterAlterationsQuestionBuilder(nPacks, 4, nWrong, firstCorrectIsQuestion: true, letterAlterationFilters: letterAlterationFilters);
                    break;
                case FastCrowdVariation.BuildWord:
                    builderParams.wordFilters.excludeColorWords = true;
                    builderParams.wordFilters.requireDrawings = true;
                    builder = new LettersInWordQuestionBuilder(nPacks, nWrong: nWrong, useAllCorrectLetters: true, parameters: builderParams);
                    break;
                case FastCrowdVariation.Word:
                    builderParams.wordFilters.excludeColorWords = true;
                    builderParams.wordFilters.requireDrawings = true;
                    builder = new RandomWordsQuestionBuilder(nPacks, nCorrect, nWrong, parameters: builderParams);
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

        public override bool IsDataMatching(ILivingLetterData data1, ILivingLetterData data2)
        {
            LetterEqualityStrictness strictness;
            switch (Variation)
            {
                case FastCrowdVariation.LetterForm:
                    strictness = LetterEqualityStrictness.WithVisualForm;
                    break;
                case FastCrowdVariation.BuildWord:
                case FastCrowdVariation.Word:
                case FastCrowdVariation.LetterName:
                case FastCrowdVariation.Counting:
                case FastCrowdVariation.Alphabet:
                    strictness = LetterEqualityStrictness.LetterOnly;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return DataMatchingHelper.IsDataMatching(data1, data2, strictness);
        }

        public override LetterDataSoundType GetVocabularySoundType()
        {
            LetterDataSoundType soundType;
            switch (Variation)
            {
                case FastCrowdVariation.LetterForm:
                    soundType = LetterDataSoundType.Name;
                    break;
                case FastCrowdVariation.BuildWord:
                case FastCrowdVariation.Word:
                case FastCrowdVariation.LetterName:
                case FastCrowdVariation.Counting:
                case FastCrowdVariation.Alphabet:
                    soundType = LetterDataSoundType.Phoneme;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return soundType;
        }

    }
}
