using System.Collections.Generic;
using Antura.Database;
using Antura.Helpers;
using Antura.Core;
using UnityEngine;

namespace Antura.Teacher
{
    /// <summary>
    /// Selects letters inside words, then shows wrong alterations
    /// * Question: Word
    /// * Correct answers: Letter contained in the word with the correct alteration
    /// * Wrong answers: Letter contained in the word with the wrong alteration
    /// * Different packs: same Letter will be in all packs, but with different alterations
    /// </summary>
    public class LetterAlterationsInWordsQuestionBuilder : IQuestionBuilder
    {
        // focus: Letters & Words
        // pack history filter: enabled
        // journey: enabled

        private int nPacksPerRound;
        private int nRounds;
        private int nWrongs;
        private int maximumWordLength;
        private WordDataCategory category;
        private bool forceUnseparatedLetters;
        private QuestionBuilderParameters parameters;
        private LetterAlterationFilters letterAlterationFilters;

        public QuestionBuilderParameters Parameters
        {
            get { return this.parameters; }
        }

        public LetterAlterationsInWordsQuestionBuilder(int nPacksPerRound, int nRounds,
            int nWrongs = 4,
            WordDataCategory category = WordDataCategory.None,
            int maximumWordLength = 20,
            bool forceUnseparatedLetters = false,
            LetterAlterationFilters letterAlterationFilters = null,
            QuestionBuilderParameters parameters = null)
        {
            if (parameters == null) parameters = new QuestionBuilderParameters();
            this.nPacksPerRound = nPacksPerRound;
            this.nRounds = nRounds;
            this.nWrongs = nWrongs;
            this.category = category;
            this.maximumWordLength = maximumWordLength;
            this.forceUnseparatedLetters = forceUnseparatedLetters;
            this.parameters = parameters;
            this.letterAlterationFilters = letterAlterationFilters;

            // Forced parameters
            this.parameters.letterFilters.excludeDiacritics = LetterFilters.ExcludeDiacritics.All;
            this.parameters.letterFilters.excludeLetterVariations = LetterFilters.ExcludeLetterVariations.All;
        }

        private List<string> previousPacksIDs_words = new List<string>();
        private List<string> previousPacksIDs_letters = new List<string>();

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            // HACK: the game may need unseparated letters
            if (forceUnseparatedLetters)
            {
                AppManager.I.VocabularyHelper.ForceUnseparatedLetters = true;
            }

            previousPacksIDs_words.Clear();
            previousPacksIDs_letters.Clear();
            var packs = new List<QuestionPackData>();

            for (int round_i = 0; round_i < nRounds*nPacksPerRound; round_i++)
            {
                packs.Add(CreateSingleQuestionPackData());
                //Debug.LogWarning("PACK ADDED: " + packs[round_i]);
            }

            return packs;
        }

        private QuestionPackData CreateSingleQuestionPackData()
        {
            var teacher = AppManager.I.Teacher;

            // First, choose a letter (base, due to filters)
            var eligibleLetters = teacher.VocabularyAi.SelectData(
                () => FindLettersThatAppearInWords(minTimesAppearing: 1, maxWordLength: maximumWordLength),
                    new SelectionParameters(parameters.correctSeverity, 1, useJourney: parameters.useJourneyForCorrect,
                        packListHistory: parameters.correctChoicesHistory, filteringIds: previousPacksIDs_letters)
            );
            var baseLetters = eligibleLetters;

            // Choose one letter randomly from the eligible ones
            var correctAlteration = eligibleLetters.RandomSelectOne();

            //Debug.Log("Correct alteration: " + correctAlteration);
            // Find a word with the letter (strict)
            var usableWords = teacher.VocabularyAi.SelectData(
                () => FindWordsWithLetter(correctAlteration, maximumWordLength),
                    new SelectionParameters(parameters.correctSeverity, 1, useJourney: parameters.useJourneyForCorrect,
                        packListHistory: parameters.correctChoicesHistory, filteringIds: previousPacksIDs_words)
            );
            var question = usableWords[0];

            // Place the correct alteration in the correct list
            var correctAnswers = new List<LetterData>();
            correctAnswers.Add(correctAlteration);

            // Place some alterations in the wrong list
            var alterationsPool = AppManager.I.VocabularyHelper.GetAllLetterAlterations(baseLetters, letterAlterationFilters);
            var wrongAnswers = new List<LetterData>();
            //Debug.Log("N Alterations before remove correct: " + alterationsPool.Count + " " + alterationsPool.ToDebugString());

            // Remove the correct alteration (making sure to get the actual form)
            for (int i = 0; i < alterationsPool.Count; i++)
            {
                if (alterationsPool[i].IsSameLetterAs(correctAlteration, LetterEqualityStrictness.WithActualForm))
                {
                    alterationsPool.RemoveAt(i);
                }
            }

            //Debug.Log("N Alterations after remove correct: " + alterationsPool.Count + " " + alterationsPool.ToDebugString());
            wrongAnswers.AddRange(alterationsPool.RandomSelect(nWrongs));

            if (ConfigAI.VerboseQuestionPacks)
            {
                string debugString = "--------- TEACHER: question pack result ---------";
                debugString += "\nQuestion: " + question;
                debugString += "\nCorrect Answers: " + correctAnswers.Count;
                foreach (var l in correctAnswers) { debugString += " " + l; }
                ConfigAI.AppendToTeacherReport(debugString);
            }

            return QuestionPackData.Create(question, correctAnswers, wrongAnswers);
        }

        List<LetterData> lettersThatAppearInWords = new List<LetterData>();

        List<LetterData> FindLettersThatAppearInWords(int minTimesAppearing, int maxWordLength)
        {
            var vocabularyHelper = AppManager.I.VocabularyHelper;
            var eligibleLetters = new List<LetterData>();

            if (lettersThatAppearInWords.Count == 0)
            {
                var allWords = AppManager.I.Teacher.VocabularyAi.SelectData(
                    () => vocabularyHelper.GetWordsByCategory(category, parameters.wordFilters),
                        new SelectionParameters(parameters.correctSeverity, getMaxData: true, useJourney: parameters.useJourneyForCorrect,
                            packListHistory: parameters.correctChoicesHistory, filteringIds: previousPacksIDs_words));
                //Debug.Log("All words: " + allWords.Count);

                // The chosen letter should actually have words that contain it multiple times.
                // This can be quite slow, so we do this only at the start.
                var baseLetters = vocabularyHelper.GetAllLetters(parameters.letterFilters);
                var allLettersWithForms = vocabularyHelper.GetAllLetterAlterations(baseLetters, LetterAlterationFilters.FormsOfMultipleLetters);
                foreach (var letterData in allLettersWithForms)
                {
                    int nTimesAppearing = 0;
                    foreach (var wordData in allWords)
                    {
                        if (WordContainsLetter(wordData, letterData, maxWordLength))
                        {
                            nTimesAppearing++;
                            break;
                        }
                    }
                    if (nTimesAppearing >= minTimesAppearing)
                    {
                        lettersThatAppearInWords.Add(letterData);
                        //Debug.Log("Letter " + letterData + " is cool as it appears " + nTimesAppearing);
                    }
                }
                //Debug.Log("Eligible letters: " + lettersThatAppearInWords.Count);
            }

            eligibleLetters = lettersThatAppearInWords;

            return eligibleLetters;
        }

        Dictionary<LetterData, List<WordData>> eligibleWordsForLetters = new Dictionary<LetterData, List<WordData>>(new StrictLetterDataComparer(LetterEqualityStrictness.WithActualForm));

        private List<WordData> FindWordsWithLetter(LetterData containedLetter, int maxWordLength)
        {
            var vocabularyHelper = AppManager.I.VocabularyHelper;
            var eligibleWords = new List<WordData>();

            if (!eligibleWordsForLetters.ContainsKey(containedLetter))
            {
                foreach (var word in vocabularyHelper.GetWordsByCategory(category, parameters.wordFilters))
                {
                    if (!WordContainsLetter(word, containedLetter, maxWordLength)) continue;
                    eligibleWords.Add(word);
                    //Debug.Log("Letter: " + containedLetter + " in Word: " + word);
                }
                eligibleWordsForLetters[containedLetter] = eligibleWords;
            }
            eligibleWords = eligibleWordsForLetters[containedLetter];

            //Debug.LogWarning("Eligible words: " + eligibleWords.Count + " out of " + vocabularyHelper.GetWordsByCategory(category, parameters.wordFilters).Count);
            return eligibleWords;
        }

        private bool WordContainsLetter(WordData word, LetterData letter, int maxWordLength)
        {
            // Check max length
            if (word.Letters.Length > maxWordLength)
            {
                return false;
            }

            // Check that it contains the letter at least once
            if (AppManager.I.VocabularyHelper.WordContainsLetterTimes(word, letter, LetterEqualityStrictness.WithActualForm) >= 1)
            {
                //Debug.Log("Letter " + letter + " is in word " + word + " " + AppManager.I.VocabularyHelper.WordContainsLetterTimes(word, letter, LetterEqualityStrictness.WithActualForm) + " times");
                return true;
            }

            return false;
        }

    }
}