using Antura.Core;
using Antura.Database;
using System;
using System.Collections.Generic;


namespace Antura.Teacher
{
    /// <summary>
    /// Selects words given a letter
    /// * Question: The letter
    /// * Correct answers: Words with the letter
    /// * Wrong answers: Words without the letter
    /// </summary>
    public class WordsWithLetterQuestionBuilder : IQuestionBuilder
    {
        // focus: Words & Letters
        // pack history filter: enabled
        // journey: enabled

        private int nRounds;
        private int nPacksPerRound;
        private int nCorrect;
        private int nWrong;
        private bool packsUsedTogether;
        private bool forceUnseparatedLetters;
        private QuestionBuilderParameters parameters;

        public QuestionBuilderParameters Parameters
        {
            get { return this.parameters; }
        }

        public WordsWithLetterQuestionBuilder(
            int nRounds, int nPacksPerRound = 1, int nCorrect = 1, int nWrong = 0,
            bool forceUnseparatedLetters = false,
              QuestionBuilderParameters parameters = null)
        {
            if (parameters == null) parameters = new QuestionBuilderParameters();
            this.nRounds = nRounds;
            this.nPacksPerRound = nPacksPerRound;
            this.packsUsedTogether = nPacksPerRound > 1;
            this.nCorrect = nCorrect;
            this.nWrong = nWrong;
            this.forceUnseparatedLetters = forceUnseparatedLetters;
            this.parameters = parameters;
        }

        private List<string> previousPacksIDs_letters = new List<string>();
        private List<string> previousPacksIDs_words = new List<string>();

        private List<LetterData> currentRound_letters = new List<LetterData>();
        private List<WordData> currentRound_words = new List<WordData>();

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            // HACK: the game may need unseparated letters
            if (forceUnseparatedLetters)
            {
                AppManager.I.VocabularyHelper.ForceUnseparatedLetters = true;
            }

            previousPacksIDs_letters.Clear();
            previousPacksIDs_words.Clear();
            List<QuestionPackData> packs = new List<QuestionPackData>();

            for (int round_i = 0; round_i < nRounds; round_i++)
            {
                // At each round, we must make sure to not repeat some words / letters
                currentRound_letters.Clear();
                currentRound_words.Clear();

                for (int pack_i = 0; pack_i < nPacksPerRound; pack_i++)
                {
                    packs.Add(CreateSingleQuestionPackData(pack_i));
                }
            }
            return packs;
        }

        private QuestionPackData CreateSingleQuestionPackData(int inRoundPackIndex)
        {
            var teacher = AppManager.I.Teacher;

            bool useJourneyForLetters = parameters.useJourneyForCorrect;
            // @note: we also force the journey if the packs must be used together, as the data filters for journey clash with the new filter
            if (packsUsedTogether)
            {
                useJourneyForLetters = false;
            }

            // Get a letter
            var usableLetters = teacher.VocabularyAi.SelectData(
              () => FindLettersThatAppearInWords(atLeastNWords: nCorrect),
                new SelectionParameters(parameters.correctSeverity, 1, useJourney: useJourneyForLetters,
                        packListHistory: parameters.correctChoicesHistory, filteringIds: previousPacksIDs_letters));
            var commonLetter = usableLetters[0];
            currentRound_letters.Add(commonLetter);

            // Get words with the letter 
            // (but without the previous letters)
            var correctWords = teacher.VocabularyAi.SelectData(
                () => FindWordsWithLetter(commonLetter),
                    new SelectionParameters(parameters.correctSeverity, nCorrect, useJourney: parameters.useJourneyForCorrect,
                        packListHistory: parameters.correctChoicesHistory, filteringIds: previousPacksIDs_words));
            currentRound_words.AddRange(correctWords);

            // Get words without the letter (only for the first pack of a round)
            var wrongWords = new List<WordData>();
            if (inRoundPackIndex == 0)
            {
                wrongWords = teacher.VocabularyAi.SelectData(
                    () => FindWrongWords(correctWords),
                        new SelectionParameters(parameters.wrongSeverity, nWrong, useJourney: parameters.useJourneyForWrong,
                            journeyFilter: SelectionParameters.JourneyFilter.CurrentJourney));
                currentRound_words.AddRange(wrongWords);
            }

            var pack = QuestionPackData.Create(commonLetter, correctWords, wrongWords);

            if (ConfigAI.VerboseQuestionPacks)
            {
                string debugString = "--------- TEACHER: question pack result ---------";
                debugString += "\nQuestion: " + commonLetter;
                debugString += "\nCorrect Answers: " + correctWords.Count;
                foreach (var l in correctWords) debugString += " " + l;
                debugString += "\nWrong Answers: " + wrongWords.Count;
                foreach (var l in wrongWords) debugString += " " + l;
                ConfigAI.AppendToTeacherReport(debugString);
            }

            return pack;
        }

        private List<LetterData> FindLettersThatAppearInWords(int atLeastNWords)
        {
            var eligibleLetters = new List<LetterData>();
            var vocabularyHelper = AppManager.I.VocabularyHelper;
            var allLetters = vocabularyHelper.GetAllLetters(parameters.letterFilters);
            var badWords = new List<WordData>(currentRound_words);
            foreach (var letter in allLetters)
            {
                // Check number of words
                var wordsWithLetterFull = vocabularyHelper.GetWordsWithLetter(parameters.wordFilters, letter, parameters.letterEqualityStrictness);
                wordsWithLetterFull.RemoveAll(x => badWords.Contains(x));  // Remove the already used words
                wordsWithLetterFull.RemoveAll(x => vocabularyHelper.ProblematicWordIds.Contains(x.Id));  // HACK: Skip the problematic words (for now)

                if (wordsWithLetterFull.Count == 0)
                {
                    continue;
                }

                var wordsWithLetter = AppManager.I.Teacher.VocabularyAi.SelectData(
                    () => wordsWithLetterFull,
                        new SelectionParameters(SelectionSeverity.AsManyAsPossible, getMaxData: true, useJourney: true), canReturnZero: true);

                int nWords = wordsWithLetter.Count;

                if (nWords < atLeastNWords)
                {
                    continue;
                }

                // Avoid using letters that already appeared in the current round's words
                if (packsUsedTogether && vocabularyHelper.AnyWordContainsLetter(letter, currentRound_words))
                {
                    continue;
                }

                //UnityEngine.Debug.Log("OK letter: " + letter + " with nwords: "  + wordsWithLetter.Count);

                eligibleLetters.Add(letter);
            }
            return eligibleLetters;
        }

        private List<WordData> FindWordsWithLetter(LetterData commonLetter)
        {
            var eligibleWords = new List<WordData>();
            var vocabularyHelper = AppManager.I.VocabularyHelper;
            var words = vocabularyHelper.GetWordsWithLetter(parameters.wordFilters, commonLetter, parameters.letterEqualityStrictness);
            var badLetters = new List<LetterData>(currentRound_letters);
            badLetters.Remove(commonLetter);
            foreach (var w in words)
            {
                if (vocabularyHelper.ProblematicWordIds.Contains(w.Id))
                {
                    // HACK: Skip the problematic words (for now)
                    continue;
                }

                // Not words that have one of the previous letters (but the current one)
                if (vocabularyHelper.WordContainsAnyLetter(w, badLetters))
                {
                    continue;
                }

                eligibleWords.Add(w);
            }
            return eligibleWords;
        }

        private List<WordData> FindWrongWords(List<WordData> correctWords)
        {
            var eligibleWords = new List<WordData>();
            var vocabularyHelper = AppManager.I.VocabularyHelper;
            var words = vocabularyHelper.GetWordsNotIn(parameters.wordFilters, correctWords.ToArray());
            var badLetters = new List<LetterData>(currentRound_letters);
            foreach (var w in words)
            {
                // Not words that have one of the previous letters
                if (vocabularyHelper.WordContainsAnyLetter(w, badLetters))
                {
                    continue;
                }

                eligibleWords.Add(w);
            }
            return eligibleWords;
        }

    }
}