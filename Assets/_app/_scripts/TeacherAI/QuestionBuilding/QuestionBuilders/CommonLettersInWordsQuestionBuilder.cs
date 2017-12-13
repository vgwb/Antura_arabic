using System.Collections.Generic;
using Antura.Core;
using Antura.Database;
using Antura.Helpers;

namespace Antura.Teacher
{
    /// <summary>
    /// Selects words that have letters in commons.
    /// * Question: Words with letters in common
    /// * Correct answers: letters in common
    /// * Wrong answers: letters not in common
    /// @note: this now uses Strictness to define whether the common letters must have the same form or not
    /// </summary>
    public class CommonLettersInWordQuestionBuilder : IQuestionBuilder
    {
        // focus: Letters & Words
        // pack history filter: DISABLED - the special logic needed makes it really hard to use a pack history filter here
        // journey: enabled

        private int nPacks;
        private int nMinCommonLetters;
        private int nMaxCommonLetters;
        private int nWrong;
        private int nWords;
        private LetterEqualityStrictness letterEqualityStrictness;
        private QuestionBuilderParameters parameters;

        public QuestionBuilderParameters Parameters
        {
            get { return this.parameters; }
        }

        public CommonLettersInWordQuestionBuilder(int nPacks, int nMinCommonLetters = 1, int nMaxCommonLetters = 1, int nWrong = 0, int nWords = 1,
            LetterEqualityStrictness letterEqualityStrictness = LetterEqualityStrictness.LetterOnly,
            QuestionBuilderParameters parameters = null)
        {
            if (parameters == null) parameters = new QuestionBuilderParameters();
            this.nPacks = nPacks;
            this.nMinCommonLetters = nMinCommonLetters;
            this.nMaxCommonLetters = nMaxCommonLetters;
            this.nWrong = nWrong;
            this.nWords = nWords;
            this.letterEqualityStrictness = letterEqualityStrictness;
            this.parameters = parameters;
        }

        private List<string> previousPacksIDs = new List<string>();

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            previousPacksIDs.Clear();
            var packs = new List<QuestionPackData>();
            for (int pack_i = 0; pack_i < nPacks; pack_i++)
            {
                packs.Add(CreateSingleQuestionPackData());
            }
            return packs;
        }

        private QuestionPackData CreateSingleQuestionPackData()
        {
            QuestionPackData pack = null;
            var teacher = AppManager.I.Teacher;
            var vocabularyHelper = AppManager.I.VocabularyHelper;

            // @note this specific builder works differently, because we first need to get words and then their letters
            // this is a special case because the focus in both on words and on letters
            // we should try to modify it to mimic the rest of the system, i.e. by first selecting letters, and only then selecting words
            // however, we cannot do so easily, as we cannot just get words that 'contain letters that may be common'
            // instead, I should just count common letters, and then select these letters that appear more than nWords*nPacks times

            // Get all words
            var allWords = teacher.VocabularyAi.SelectData(
                () => vocabularyHelper.GetAllWords(parameters.wordFilters),
                    new SelectionParameters(parameters.correctSeverity, getMaxData: true, useJourney: parameters.useJourneyForCorrect));

            int nAttempts = 100;
            bool found = false;
            while (nAttempts > 0 && !found)
            {
                var wordsToUse = allWords.RandomSelect(nWords);
                var commonLetters = vocabularyHelper.GetCommonLettersInWords(letterEqualityStrictness, wordsToUse.ToArray());
                //UnityEngine.Debug.Log("Common letters for words " + wordsToUse.ToDebugString() + " are " + commonLetters.ToDebugString());
                if (commonLetters.Count < nMinCommonLetters || commonLetters.Count > nMaxCommonLetters)
                {
                    nAttempts--;
                    continue;
                }

                var nonCommonLetters = vocabularyHelper.GetLettersNotIn(letterEqualityStrictness, parameters.letterFilters, commonLetters.ToArray()).RandomSelect(nWrong);
                UnityEngine.Debug.Log("Common letters: " + commonLetters.ToDebugString() + " non-common: " + nonCommonLetters.ToDebugString());

                // Debug
                if (ConfigAI.VerboseQuestionPacks)
                {
                    string debugString = "--------- TEACHER: question pack result ---------";
                    debugString += "\nCommon letters: ";
                    foreach (var l in commonLetters)
                    {
                        debugString += " " + l;
                    }
                    foreach (var word in wordsToUse)
                    {
                        debugString += "\nWord: " + word;
                        foreach (var l in word.Letters) debugString += " " + l;
                    }
                    ConfigAI.AppendToTeacherReport(debugString);
                }

                pack = QuestionPackData.Create(wordsToUse, commonLetters, nonCommonLetters);
                found = true;
            }

            if (!found)
            {
                throw new System.Exception("Could not find enough data to prepare the CommonLettersInWordQuestionBuilder. (Special behaviour)");
            }

            return pack;
        }


    }
}