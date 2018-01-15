using System.Collections.Generic;
using System.Linq;
using Antura.Core;
using Antura.Database;
using Antura.Helpers;
using UnityEngine;

namespace Antura.Teacher
{
    /// <summary>
    /// Selects words that have one letter in common.
    /// * Question: Words with letter in common
    /// * Correct answers: letter in common
    /// * Wrong answers: letters not in common
    /// @note: this now uses Strictness to define whether the common letters must have the same form or not
    /// @note: this has been rewritten following WordsWithLetter and simplified
    /// </summary>
    public class CommonLetterInWordQuestionBuilder : IQuestionBuilder
    {
        // focus: Letters & Words
        // pack history filter: DISABLED - the special logic needed makes it really hard to use a pack history filter here
        // journey: enabled

        private int nPacks;
        private int nCorrect;
        private int nWrong;
        private int nWords;
        private LetterEqualityStrictness letterEqualityStrictness;
        private QuestionBuilderParameters parameters;

        public QuestionBuilderParameters Parameters
        {
            get { return this.parameters; }
        }

        public CommonLetterInWordQuestionBuilder(int nPacks, 
            int nCorrect = 1,
            int nWrong = 0, int nWords = 1,
            LetterEqualityStrictness letterEqualityStrictness = LetterEqualityStrictness.LetterOnly,
            QuestionBuilderParameters parameters = null)
        {
            if (parameters == null) parameters = new QuestionBuilderParameters();
            this.nPacks = nPacks;
            this.nCorrect = nCorrect;
            this.nWrong = nWrong;
            this.nWords = nWords;
            this.letterEqualityStrictness = letterEqualityStrictness;
            this.parameters = parameters;
        }

        private List<string> previousPacksIDs_letters = new List<string>();
        private List<string> previousPacksIDs_words = new List<string>();

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            previousPacksIDs_letters.Clear();
            previousPacksIDs_words.Clear();
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

            // Get a letter
            var usableLetters = teacher.VocabularyAi.SelectData(
              () => FindLettersThatAppearInWords(atLeastNWords: nWords),
                new SelectionParameters(parameters.correctSeverity, nCorrect, useJourney: parameters.useJourneyForCorrect,
                        packListHistory: parameters.correctChoicesHistory, filteringIds: previousPacksIDs_letters));
            var commonLetter = usableLetters[0];
            var correctLetters = new List<LetterData>();
            correctLetters.Add(commonLetter);
            //Debug.Log("Corrects: " + correctLetters.ToDebugString());

            // Get words with the common letter 
            // (but without the previous letters)
             var wordsWithCommonLetter = teacher.VocabularyAi.SelectData(
                () => FindWordsWithCommonLetter(commonLetter),
                    new SelectionParameters(parameters.correctSeverity, nWords, useJourney: parameters.useJourneyForCorrect,
                        packListHistory: parameters.correctChoicesHistory, filteringIds: previousPacksIDs_words));
            //Debug.Log("Words: " + wordsWithCommonLetter.ToDebugString());

            // Get letters that are not in common in both words
            var lettersNotInCommon = teacher.VocabularyAi.SelectData(
                () => FindLettersNotInCommon(wordsWithCommonLetter),
                    new SelectionParameters(parameters.wrongSeverity, nWrong, useJourney: parameters.useJourneyForWrong,
                        journeyFilter: SelectionParameters.JourneyFilter.CurrentJourney, 
                        getMaxData:true // needed to skip priority filtering, which will filter out forms!
                        ));
            lettersNotInCommon = lettersNotInCommon.RandomSelect(nWrong);  // needed to skip priority filtering, which will filter out forms!
            //Debug.Log("Not in common: " + FindLettersNotInCommon(wordsWithCommonLetter).ToDebugStringNewline());
            //Debug.Log("Wrongs: " + lettersNotInCommon.ToDebugString());

            pack = QuestionPackData.Create(wordsWithCommonLetter, correctLetters, lettersNotInCommon);

            if (ConfigAI.VerboseQuestionPacks)
            {
                string debugString = "--------- TEACHER: question pack result ---------";
                debugString += "\nQuestion: " + wordsWithCommonLetter.ToDebugString();
                debugString += "\nCorrect Answers: " + correctLetters.Count;
                foreach (var l in correctLetters) debugString += " " + l;
                debugString += "\nWrong Answers: " + lettersNotInCommon.Count;
                foreach (var l in lettersNotInCommon) debugString += " " + l;
                ConfigAI.AppendToTeacherReport(debugString);
            }

            return pack;
        }


        private Dictionary<LetterData, List<WordData>> lettersToWordCache;

        private List<LetterData> FindLettersThatAppearInWords(int atLeastNWords)
        {
            var eligibleLetters = new List<LetterData>();
            var vocabularyHelper = AppManager.I.VocabularyHelper;

            if (lettersToWordCache == null)
            {
                lettersToWordCache = new Dictionary<LetterData, List<WordData>>(new StrictLetterDataComparer(letterEqualityStrictness));

                var allLetters = vocabularyHelper.GetAllLettersAndForms(parameters.letterFilters);     // we consider different forms as different letters
                foreach (var letter in allLetters)
                {
                    lettersToWordCache[letter] = new List<WordData>();
                    // Check number of words
                    var wordsWithLetterForm = vocabularyHelper.GetWordsWithLetter(parameters.wordFilters, letter, letterEqualityStrictness);
                    //Debug.Log("N words for letter " + letter + " is " + wordsWithLetterFull.Count);
                    wordsWithLetterForm.RemoveAll(x => vocabularyHelper.ProblematicWordIds.Contains(x.Id));  // HACK: Skip the problematic words (for now)

                    lettersToWordCache[letter].AddRange(wordsWithLetterForm);
                }
            }

            foreach (var letter in lettersToWordCache.Keys)
            {
                var words = lettersToWordCache[letter];

                if (words.Count == 0)  continue;

                // Check number of words
                var wordsWithLetter = AppManager.I.Teacher.VocabularyAi.SelectData(
                    () => words,
                        new SelectionParameters(SelectionSeverity.AsManyAsPossible, getMaxData: true, useJourney: true), canReturnZero: true);

                if (wordsWithLetter.Count < atLeastNWords)
                {
                    continue;
                }

                //UnityEngine.Debug.Log("OK letter: " + letter + " with nwords: "  + wordsWithLetter.Count);
                eligibleLetters.Add(letter);
            }

            return eligibleLetters;
        }

        private List<WordData> FindWordsWithCommonLetter(LetterData commonLetter)
        {
            var eligibleWords = new List<WordData>();
            var vocabularyHelper = AppManager.I.VocabularyHelper;
            var words = vocabularyHelper.GetWordsWithLetter(parameters.wordFilters, commonLetter, letterEqualityStrictness);
            foreach (var w in words)
            {
                if (vocabularyHelper.ProblematicWordIds.Contains(w.Id))
                {
                    // HACK: Skip the problematic words (for now)
                    continue;
                }

                eligibleWords.Add(w);
            }
            return eligibleWords;
        }

        private List<LetterData> FindLettersNotInCommon(List<WordData> words)
        {
            var vocabularyHelper = AppManager.I.VocabularyHelper;
            var lettersNotInCommon = vocabularyHelper.GetNotCommonLettersInWords(parameters.letterFilters, letterEqualityStrictness, words.ToArray());
            return lettersNotInCommon;
        }

    }
}